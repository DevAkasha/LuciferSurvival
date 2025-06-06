using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using static UnityEngine.Rendering.DebugUI;


// 상태 enum
public enum TimeState
{
    Day,
    Night
}

public class TimeManager : Singleton<TimeManager>
{
    protected override bool IsPersistent => false;

    [SerializeField] public TimeState currentTimeState; // 현재 상태

    [Header("Sun")]
    public Light sun; // 태양
    //public Color dayLightColor; // 태양 밝기
    public float dayLightIntensity = 1.0f; // 빛의 세기

    [Header("Moon")]
    public Light moon;
    public Color nightLightColor = new Color(0.2f, 0.2f, 1f);
    public float nightLightIntensity = 0.3f;

    public float dayAmbientIntensity = 1.0f; // 낮 주변광, RenderSettings.ambientIntensity에 반영
    public float nightAmbientIntensity = 0.4f; // 밤 주변광
    public float dayReflectionIntensity = 1.0f; // 낮 반사광
    public float nightReflectionIntensity = 0.4f; // 밤 반사광

    [Header("Transition Settings")]
    public float transitionDuration = 2.0f; // 상태 전환에 걸리는 시간

    [Header("Night Duration Settings")]
    [SerializeField] private bool enableNightTimer = true; // 밤->낮 타이머 활성화 여부
    [SerializeField] private float defaultNightDuration = 60f; // 데이터가 없을 경우 사용할 기본값
    private float nightDuration; // 실제 사용할 밤 지속 시간(초), 데이터테이블에서 가져옴
    private Coroutine transitionRoutine; // 전환 코루틴

    // 스테이지와 웨이브 추적 변수 추가
    [Header("Wave Tracking")]
    [SerializeField] private int currentStage = 1;
    [SerializeField] private int currentWaveCount = 0;
    [SerializeField] private int maxWaveCount = 5; // 총 5번의 전투

    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private GameObject timeIndicator;
    [SerializeField] private Image dayAndNightImage;
    private Coroutine nightRoutine;

    [Header("UI References")]
    [SerializeField] private BattleScreen battleScreen;

    private void Start()
    {
        // 초기값 설정
        nightDuration = defaultNightDuration;

        // 시작 초기값은 밤
        currentTimeState = TimeState.Day;
        ApplyLightingInstant();
    }

    // 낮 전환
    public void SetDay()
    {
        if (currentTimeState != TimeState.Day)
        {
            currentTimeState = TimeState.Day;
            WaveManager.Instance.SetKillCountListener();
            StartLightingTransition();

            if (nightRoutine != null)
                StopCoroutine(nightRoutine);

            SetDayAndNight();

            if (battleScreen != null && battleScreen.gameObject.activeInHierarchy)
                battleScreen.UpdateBattleScreenState();
        }
    }

    // 밤 전환
    public void SetNight()
    {
        if (currentTimeState != TimeState.Night)
        {
            currentTimeState = TimeState.Night;
            WaveManager.Instance.RemoveKillCountListener();
            StartLightingTransition();
            if (battleScreen != null && battleScreen.gameObject.activeInHierarchy)
                battleScreen.UpdateBattleScreenState();

            if (nightRoutine != null)
                StopCoroutine(nightRoutine);

            SetDayAndNight();

            // 밤 시계 표시 코루틴 시작
            nightRoutine = StartCoroutine(NightTimeProcess());
            SetWaveText();

            // 밤으로 전환됐으므로 밤->낮 타이머 설정
            if (enableNightTimer)
            {
                SetNightTimer();
            }
        }
    }

    private void SetNightTimer()
    {
        if (!enableNightTimer || currentTimeState != TimeState.Night)
            return;

        enableNightTimer = false;
        float duration = WaveManager.Instance.curWave?.NightTime ?? defaultNightDuration;

        this.DelayedCall(duration, () =>
        {
            if (currentTimeState == TimeState.Night)
                StageManager.Instance.ChangeToDay();

            enableNightTimer = true;
        });
    }

    private void StartLightingTransition()
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(LightTransitionCoroutine());
    }

    private IEnumerator LightTransitionCoroutine()
    {
        float timeElapsed = 0f;

        float startSunIntensity = sun.intensity;
        float targetSunIntensity = (currentTimeState == TimeState.Day) ? dayLightIntensity : 0f;

        float startMoonIntensity = moon.intensity;
        float targetMoonIntensity = (currentTimeState == TimeState.Night) ? nightLightIntensity : 0f;

        float startAmbient = RenderSettings.ambientIntensity;
        float targetAmbient = (currentTimeState == TimeState.Day) ? dayAmbientIntensity : nightAmbientIntensity;

        float startReflection = RenderSettings.reflectionIntensity;
        float targetReflection = (currentTimeState == TimeState.Day) ? dayReflectionIntensity : nightReflectionIntensity;

        Color startSunColor = sun.color;
        Color targetSunColor = (currentTimeState == TimeState.Day)
            ? new Color(1.0f, 244f / 255f, 214f / 255f)
            : new Color(0.2f, 0.2f, 0.2f);

        moon.gameObject.SetActive(true); // 항상 켜두고 밝기만 조절

        while (timeElapsed < transitionDuration)
        {
            float t = timeElapsed / transitionDuration;

            sun.intensity = Mathf.Lerp(startSunIntensity, targetSunIntensity, t);
            sun.color = Color.Lerp(startSunColor, targetSunColor, t);

            moon.intensity = Mathf.Lerp(startMoonIntensity, targetMoonIntensity, t);

            RenderSettings.ambientIntensity = Mathf.Lerp(startAmbient, targetAmbient, t);
            RenderSettings.reflectionIntensity = Mathf.Lerp(startReflection, targetReflection, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // 보정
        sun.intensity = targetSunIntensity;
        sun.color = targetSunColor;

        moon.intensity = targetMoonIntensity;
        moon.gameObject.SetActive(currentTimeState == TimeState.Night);

        RenderSettings.ambientIntensity = targetAmbient;
        RenderSettings.reflectionIntensity = targetReflection;

        transitionRoutine = null;
    }

    // 조명 즉시적용
    private void ApplyLightingInstant()
    {
        sun.intensity = (currentTimeState == TimeState.Day) ? dayLightIntensity : 0f;
        sun.color = (currentTimeState == TimeState.Day)
            ? new Color(1.0f, 244f / 255f, 214f / 255f)
            : new Color(0.2f, 0.2f, 0.2f);

        moon.intensity = (currentTimeState == TimeState.Night) ? nightLightIntensity : 0f;
        moon.color = nightLightColor;
        moon.gameObject.SetActive(currentTimeState == TimeState.Night);

        RenderSettings.ambientIntensity = (currentTimeState == TimeState.Day) ? dayAmbientIntensity : nightAmbientIntensity;
        RenderSettings.reflectionIntensity = (currentTimeState == TimeState.Day) ? dayReflectionIntensity : nightReflectionIntensity;
    }

    public void SetDayInfoText(int value)
    {
        infoText.text = $"{value}/{WaveManager.Instance.CalculateAllCount()}";
    }

    public void SetDayAndNight()
    {
        if (currentTimeState == TimeState.Day)
        {
            timeIndicator.SetActive(true);
            dayAndNightImage.gameObject.SetActive(false);
        }
        else
        {
            timeIndicator.SetActive(false);
            dayAndNightImage.gameObject.SetActive(true);
        }
    }

    public void SetWaveText()
    {
        waveText.text = $"{StageManager.Instance.waveRound + 1}/5";
    }

    private IEnumerator NightTimeProcess()
    {
        int totalMinutesInDay = 24 * 60;      // 하루 1440분
        float nightStartMinute = 19f * 60f;    // 19:00 → 1140분
        float nightSpanMinutes = 12f * 60f;    // 12시간 → 720분

        int steps = Mathf.CeilToInt(nightDuration);
        float stepDuration = nightDuration / steps;

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;

            // 디스크 회전 (19시→7시 구간에 걸쳐 0°→180°)
            float angle = Mathf.Lerp(0f, 180f, t);
            dayAndNightImage.rectTransform.localEulerAngles = new Vector3(0, 0, -angle + 90);

            yield return new WaitForSeconds(stepDuration);
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(TimeManager))]
public class TimeChanger : Editor
{
    public override void OnInspectorGUI() // 밤낮바꿈버튼
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("낮바뀜"))
            {
                ((TimeManager)target).SetDay();
            }

            if (GUILayout.Button("밤바뀜"))
            {
                ((TimeManager)target).SetNight();
            }
        }
    }
}
#endif