using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ironcow;

public class TimeScreen : TimeManager
{
    public TMP_Text timeText; // ������ ������ �˰����ִ� �ؽ�Ʈ
    public TMP_Text timerText; // ���϶� �����ð� �����ִ� �ؽ�Ʈ

    // ������Ʈ�� Ȱ��ȭ�ɶ�
    private void OnEnable()
    {
        OnDay += UpdateUI;
        OnNight += UpdateUI;
    }

    // ������Ʈ�� ��Ȱ��ȭ�ɶ�, �޸� ������ ���ؼ� ������ �̺�Ʈ ����
    private void OnDisable()
    {
        OnDay -= UpdateUI;
        OnNight -= UpdateUI;
    }

    private void UpdateUI()
    {
        bool isNight = IsNight(); // ���϶��� Ÿ�̸Ӱ� ���̰�
        timerText.gameObject.SetActive(isNight);

        if (isNight)
        {
            float leftTime = NightTimeLeft;
            timerText.text = $"���� �ð�: {leftTime:F1}��"; // �� ���� �ð� ǥ��
        }

        timeText.text = IsNight() ? "��" : "��";
    }
}
