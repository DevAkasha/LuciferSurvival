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

    private int slotIndex;

    public void SetSlot(UnitDataSO unitDataSO, int index, bool isPurchase = false)
    {
        unitSprite.sprite = unitDataSO.thumbnail;  
        soldImage.gameObject.SetActive(false);
        unitModel = new UnitModel(unitDataSO);
        //유닛, 등급 이미지를 아직 구하지 않아 생성자에 적용할지 확인이 필요함
        unitNameText.text = unitDataSO.displayName;
        unitCostText.text = unitDataSO.cost.ToString();
        slotIndex = index;
        if (isPurchase)
        {
            soldImage.gameObject.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //구매 여부 체크
        if (SummonTableUtil.purchaseList[slotIndex] == false)
        {
            //구매가능 체크
            if (UnitManager.Instance.CanAddUnit(unitModel) && StageManager.Instance.ReduceSoulStone(unitModel.cost))
            {
                SummonTableUtil.purchaseList[slotIndex] = true;
                UnitManager.Instance.AddUnit(unitModel);
                soldImage.gameObject.SetActive(true);
                StageUIManager.Instance.RefreshAllUnitSlots();
            }
        }
    }
}
