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

    public Lock unitLock;

    public void SetSlot(UnitDataSO unitDataSO)
    {
        //유닛, 등급 이미지를 아직 구하지 않아 생성자에 적용할지 확인이 필요함
        unitNameText.text = unitDataSO.displayName;
        unitCostText.text = unitDataSO.cost.ToString();
    }
}
