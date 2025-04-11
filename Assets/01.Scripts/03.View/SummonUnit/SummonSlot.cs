using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummonSlot : MonoBehaviour
{
    [SerializeField]
    private Image unitSprite;

    [SerializeField]
    private Image gradeSprite;

    [SerializeField]
    private TextMeshProUGUI unitNameText;

    [SerializeField]
    private TextMeshProUGUI unitDescText;

    [SerializeField]
    private TextMeshProUGUI unitCostText;

    public void SetSlot(UnitDataSO unitDataSO)
    {
        //����, ��� �̹����� ���� ������ �ʾ� �����ڿ� �������� Ȯ���� �ʿ���
        unitNameText.text = unitDataSO.displayName;
        unitDescText.text = unitDataSO.description;
        unitCostText.text = unitDataSO.cost.ToString();
    }
}
