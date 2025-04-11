using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScreen : TimeManager
{
    [SerializeField] private Text timeText;

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
        //timeText.text = IsNight() ? "밤" : "낮";
    }
}
