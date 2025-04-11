using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScreen : TimeManager
{
    [SerializeField] private Text timeText;

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
        //timeText.text = IsNight() ? "��" : "��";
    }
}
