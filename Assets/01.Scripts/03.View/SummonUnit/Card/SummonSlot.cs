using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SummonSlot : MonoBehaviour, IPointerClickHandler
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

    [SerializeField]
    private Image soldImage;

    public Lock unitLock;

    private UnitModel unitModel;

    public void SetSlot(UnitDataSO unitDataSO)
    {
        soldImage.gameObject.SetActive(false);
        unitModel = new UnitModel(unitDataSO);
        //유닛, 등급 이미지를 아직 구하지 않아 생성자에 적용할지 확인이 필요함
        unitNameText.text = unitDataSO.displayName;
        unitCostText.text = unitDataSO.cost.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (StageManager.Instance.CanAddUnit(unitModel) && StageManager.Instance.UseSoulStone(unitModel.cost))
        {
            StageManager.Instance.AddUnit(unitModel);
            soldImage.gameObject.SetActive(true);
            StageUIManager.Instance.RefreshAllUnitSlots();
        }
    }
}
