using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 상태 enum
public enum TimeState
{
    Day,
    Night
}

public class TimeManager : Singleton<TimeManager>
{
    protected override bool IsPersistent => false;

    [SerializeField] private TimeState currentTimeState; // 현재 상태

    [Header("Sun")]
    public Light sun; // 태양
    //public Color dayLightColor; // 태양 밝기, 쓸모없으면 나중에 제외
    public float dayLightIntensity = 1.0f; // 빛의 세기

    [Header("Moon")]
    public Light moon;
    public Color nightLightColor = new Color(0.2f, 0.2f, 1f);
    public float nightLightIntensity = 0.3f;

    public float dayAmbientIntensity = 1.0f; // 낮 주변광, RenderSettings.ambientIntensity에 반영
    public float nightAmbientIntensity = 0.4f; // 밤 주변광
    public float dayReflectionIntensity = 1.0f; // 낮 반사광 ?
    public float nightReflectionIntensity = 0.4f; // 밤 반사광 ?

    [Header("Transition Settings")]
    public float transitionDuration = 2.0f; // 상태 전환에 걸리는 시간

    [Header("Battle Screen Settings")]
    public Image battleScreen;
    public float battleScreenBlinkInterval = 0.5f; // 깜빡임 간격
    public float battleScreenMinAlpha = 0.2f; // 최소 투명도
    public float battleScreenMaxAlpha = 0.3f; // 최대 투명도

    [Header("Night Duration Settings")]
    [SerializeField] private bool enableNightTimer = true; // 밤->낮 타이머 활성화 여부
    [SerializeField] private float defaultNightDuration = 30f; // 데이터가 없을 경우 사용할 기본값
    private float nightDuration; // 실제 사용할 밤 지속 시간(초), 데이터테이블에서 가져옴

    // 스테이지와 웨이브 추적 변수 추가
    [Header("Wave Tracking")]
    [SerializeField] private int currentStage = 1;
    [SerializeField] private int currentWaveCount = 0;
    [SerializeField] private int maxWaveCount = 5; // 총 5번의 전투

    private Coroutine transitionRoutine; // 전환 코루틴
    private Coroutine battleScreenRoutine; // 배틀스크린 깜빡임 코루틴

    private bool isNightTimerSet = false; // 타이머 관련 변수

    private void Start()
    {
        // 초기값 설정
        nightDuration = defaultNightDuration;

        // 시작 초기값은 밤
        currentTimeState = TimeState.Night;
        ApplyLightingInstant();

        // 배틀스크린 초기화
        if (battleScreen != null)
        {
            battleScreen.color = Color.clear;
        }

        // 데이터테이블에서 nightTime 값 가져오기 (Start에서는 생략, 첫 웨이브는 기본값 사용)
        // 실제 웨이브가 시작되면 UpdateNightDurationFromWaveData()가 호출됨

        // 밤->낮 자동 전환 타이머 활성화
        //if (enableNightTimer)
        //{
        //    SetNightTimer();
        //}
    }

    private void OnEnable()
    {
        // 씬이 활성화될 때 배틀스크린 상태 확인
        UpdateBattleScreenState();
    }

    private void OnDisable()
    {
        // 씬이 비활성화될 때 코루틴 정리
        if (battleScreenRoutine != null)
        {
            StopCoroutine(battleScreenRoutine);
            battleScreenRoutine = null;
        }
    }

    /// <summary>
    /// 낮 전환
    /// </summary>
    public void SetDay()
    {
        if (currentTimeState != TimeState.Day)
        {
            currentTimeState = TimeState.Day;
            StartLightingTransition();

            UpdateBattleScreenState();
        }
    }

    /// <summary>
    /// 밤 전환
    /// </summary>
    public void SetNight()
    {
        if (currentTimeState != TimeState.Night)
        {
            currentTimeState = TimeState.Night;
            StartLightingTransition();
            UpdateBattleScreenState();

            // 밤으로 전환됐으므로 밤->낮 타이머 설정
            if (enableNightTimer)
            {
                SetNightTimer();
            }
        }
    }

    private void SetNightTimer()
    {
        if (!isNightTimerSet && currentTimeState == TimeState.Night)
        {
            isNightTimerSet = true;
            Debug.Log($"{WaveManager.Instance.WaveData.nightTime}초 타이머 시작");
            UnityTimer.ScheduleRepeating(WaveManager.Instance.WaveData.nightTime, () =>
            {
                // 타이머가 완료되면 낮으로 전환
                if (currentTimeState == TimeState.Night)
                {
                    GameManager.Instance.ExhangeToDay();
                }
                isNightTimerSet = false;
            });
        }
    }

    private void UpdateBattleScreenState()
    {
        if (battleScreen == null) return;

        // 이미 실행 중인 코루틴이 있으면 중지
        if (battleScreenRoutine != null)
        {
            StopCoroutine(battleScreenRoutine);
            battleScreenRoutine = null;
            battleScreen.color = Color.clear; // 투명하게 초기화
        }

        // 낮 상태일 때만 깜빡임 시작
        if (currentTimeState == TimeState.Day)
        {
            battleScreen.gameObject.SetActive(true);
            battleScreenRoutine = StartCoroutine(ShowBattleScreen());
        }
        else
        {
            battleScreen.gameObject.SetActive(false); // 밤엔 비활성화
        }
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

    // 배틀스크린 깜빡임 효과
    private IEnumerator ShowBattleScreen()
    {
        float alpha = battleScreenMinAlpha;
        float direction = 1f; // 1이면 증가, -1이면 감소

        Color baseColor = new Color(1f, 0f, 0f);

        while (currentTimeState == TimeState.Day)
        {
            // 알파값 증가 또는 감소
            alpha += direction * Time.deltaTime * 0.2f; // 속도 조절 (0.2는 조정 가능)

            // 방향 반전 조건
            if (alpha >= battleScreenMaxAlpha)
            {
                alpha = battleScreenMaxAlpha;
                direction = -1f;
            }
            else if (alpha <= battleScreenMinAlpha)
            {
                alpha = battleScreenMinAlpha;
                direction = 1f;
            }

            battleScreen.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null; // 매 프레임 갱신
        }

        battleScreen.color = Color.clear;
    }
}

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