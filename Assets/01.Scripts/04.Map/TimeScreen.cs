using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ironcow;

public class TimeScreen : TimeManager
{
    public TMP_Text timeText; // 밤인지 낮인지 알게해주는 텍스트
    public TMP_Text timerText; // 밤일때 남은시간 보여주는 텍스트

    // 오브젝트가 활성화될때
    private void OnEnable()
    {
        OnDay += UpdateUI;
        OnNight += UpdateUI;
    }

    // 오브젝트가 비활성화될때, 메모리 누수를 위해서 꺼질때 이벤트 제거
    private void OnDisable()
    {
        OnDay -= UpdateUI;
        OnNight -= UpdateUI;
    }

    private void UpdateUI()
    {
        bool isNight = IsNight(); // 밤일때만 타이머가 보이게
        timerText.gameObject.SetActive(isNight);

        if (isNight)
        {
            float leftTime = NightTimeLeft;
            timerText.text = $"남은 시간: {leftTime:F1}초"; // 밤 남은 시간 표시
        }

        timeText.text = IsNight() ? "밤" : "낮";
    }
}
