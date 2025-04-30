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

    [SerializeField] private Image damageImage;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.5f); // 빨간색 반투명

    private Coroutine transitionRoutine; // 전환 코루틴
    private Coroutine flashCoroutine; // 스크린이펙트 코루틴

    private void Start()
    {
        // 시작 초기값은 밤
        currentTimeState = TimeState.Night; 
        ApplyLightingInstant();
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

    public void FlashDamage()
    {
        if (currentTimeState == TimeState.Day) // 낮일 때만
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);

            damageImage.gameObject.SetActive(true);
            flashCoroutine = StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        damageImage.color = flashColor;

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            float alpha = Mathf.Lerp(flashColor.a, 0f, elapsed / flashDuration);
            damageImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        damageImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
        flashCoroutine = null;
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