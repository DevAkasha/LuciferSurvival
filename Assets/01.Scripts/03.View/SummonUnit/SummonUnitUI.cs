using System.Collections;
using System.Collections.Generic;
using Ricimi;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class SummonUnitUI : MonoBehaviour
{
    [SerializeField]
    private Transform summonSlotLayout;

    [SerializeField]
    private SummonSlot summonSlotPrefab;

    [SerializeField]
    private List<Color> gradeColors;  

    [SerializeField]
    private List<Sprite> gradeSprites;

    [SerializeField]
    private Button levelUpButton;

    [SerializeField]
    private Button rerollButton;

    [SerializeField]
    private TextMeshProUGUI rerollCostText;

    [SerializeField]
    private TextMeshProUGUI shopLevelUpCostText;

    [SerializeField]
    private TextMeshProUGUI shopLevelText;

    [SerializeField]
    private TextMeshProUGUI soulStoneCountText;

    private int shopLevel = 1;
    private int rerollCost = 3;

    private SummonSlot[] curSlots;

    private void Start()
    {
        curSlots = summonSlotLayout.GetComponentsInChildren<SummonSlot>();
        SetRandomUnit();
        UpdateShopLevelUpCost();
        UpdateRerollCost();
        UpdateSoulStone();
    }

    public void OnclickShopLevelUp()
    {
        //현재 상점 레벨 체크 후 레벨업이 가능한 재화이면 레벨업
        if (SummonTableUtil.CanLevelUp(shopLevel))
        {
            if (StageManager.Instance.UseSoulStone(SummonTableUtil.GetSummonTable(shopLevel + 1).cost))
            {

            }
        }
    }

    public void OnclickRerollUnit()
    {
        if (StageManager.Instance.UseSoulStone(rerollCost + CountLockedSlot()))
        {
            SummonTableUtil.ClearAllChildren(summonSlotLayout);
            SetRandomUnit();
        }
    }

    private void SetRandomUnit()
    {
        List<int> rerollIndices = new();

        for (int i = 0; i < curSlots.Length; i++)
        {
            if (!curSlots[i].unitLock.isLocked)
                rerollIndices.Add(i);
        }

        List<UnitDataSO> rerollUnits = SummonTableUtil.RerollShop(shopLevel, rerollIndices.Count);

        for (int i = 0; i < rerollIndices.Count; i++)
        {
            int index = rerollIndices[i];
            curSlots[index].SetSlot(rerollUnits[i]);
        }
    }

    public bool CheckRerollCost()
    {
        //현재 금액이 리롤 가격보다 높을 시 버튼 활성화
        //현재 금액이 리롤 가격보다 높지 않을시 버튼 비활성화
        return false;
    }

    public void RerollButtonEnable()
    {
        rerollButton.GetComponent<CleanButton>().Interactable = true;
    }

    public void RerollButtonDisable()
    {
        rerollButton.GetComponent<CleanButton>().Interactable = false;
    }

    public int CountLockedSlot()
    {
        SummonSlot[] slotList = transform.GetComponentsInChildren<SummonSlot>();
        int result = 0;

        foreach (SummonSlot slot in slotList)
        {
            if (slot.unitLock.isLocked)
            {
                result++;
            }
        }

        return result;
    }

    public void UpdateRerollCost()
    {
        rerollCostText.text = (rerollCost + CountLockedSlot()).ToString();
    }

    public void UpdateShopLevelUpCost()
    {
        if (SummonTableUtil.CanLevelUp(shopLevel))
        {
            shopLevelUpCostText.text = SummonTableUtil.GetSummonTable(shopLevel + 1).cost.ToString();
        }
    }

    public void UpdateSoulStone()
    {
        soulStoneCountText.text = StageManager.Instance.SoulStone.ToString();
    }
}
