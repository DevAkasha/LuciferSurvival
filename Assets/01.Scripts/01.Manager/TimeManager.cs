using System;
using UnityEngine;

// 밤/낮 enum
public enum TimeState
{
    Day,
    Night
}

public class TimeManager : Singleton<TimeManager>
{
    // 현재 상태
    [SerializeField] private TimeState currentTimeState;

    [Header("Sun")]
    public Light sun;
    public Color dayLightColor = Color.white;
    public float dayLightIntensity = 1.0f;

    [Header("Moon")]
    public Light moon;
    public Color nightLightColor = new Color(0.6f, 0.6f, 1f);
    public float nightLightIntensity = 0.3f;

    [Header("Other Lighting")]
    public float dayAmbientIntensity = 1.0f;
    public float nightAmbientIntensity = 0.1f;
    public float dayReflectionIntensity = 1.0f;
    public float nightReflectionIntensity = 0.3f;


    void Start()
    {
        currentTimeState = TimeState.Night;
        UpdateLightingForNight(); // 시작 시 밤 조명으로 설정
    }

    // 낮 세팅
    public void SetDay()
    {
        if (currentTimeState != TimeState.Day)
        {
            currentTimeState = TimeState.Day;
        }
        UpdateLightingForDay();
    }

    // 밤 세팅
    public void SetNight()
    {
        if (currentTimeState != TimeState.Night)
        {
            currentTimeState = TimeState.Night;
        }
        UpdateLightingForNight();
    }

    // 낮 조명 설정
    private void UpdateLightingForDay()
    {
        // 해 조명 설정
        if (sun != null)
        {
            sun.color = dayLightColor;
            sun.intensity = dayLightIntensity;
            sun.gameObject.SetActive(true);
        }

        // 달 조명 설정
        if (moon != null)
        {
            moon.intensity = 0;
            moon.gameObject.SetActive(false);
        }

        // 환경 조명 설정
        RenderSettings.ambientIntensity = dayAmbientIntensity;
        RenderSettings.reflectionIntensity = dayReflectionIntensity;
    }

    // 밤 조명 설정
    private void UpdateLightingForNight()
    {
        // 해 조명 설정
        if (sun != null)
        {
            sun.intensity = 0;
            sun.gameObject.SetActive(false);
        }

        // 달 조명 설정
        if (moon != null)
        {
            moon.color = nightLightColor;
            moon.intensity = nightLightIntensity;
            moon.gameObject.SetActive(true);
        }

        // 환경 조명 설정
        RenderSettings.ambientIntensity = nightAmbientIntensity;
        RenderSettings.reflectionIntensity = nightReflectionIntensity;
    }
}
