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

    private SummonSlot[] curSlots;

    private void Start()
    {
        curSlots = summonSlotLayout.GetComponentsInChildren<SummonSlot>();
        SetRandomUnit();
        CheckRerollCost();
    }

    public void OnclickShopLevelUp()
    {
        //현재 상점 레벨 체크 후 레벨업이 가능한 재화이면 레벨업
        if (SummonTableUtil.CanLevelUp(StageManager.Instance.NextShopLevel))
        {
            if (StageManager.Instance.ReduceSoulStone(SummonTableUtil.GetSummonTable(StageManager.Instance.NextShopLevel).cost))
            {
                StageManager.Instance.ShopLevel = StageManager.Instance.NextShopLevel;
            }
        }
    }

    public void OnclickRerollUnit()
    {
        if (StageManager.Instance.ReduceSoulStone(StageManager.Instance.RerollCost))
        {
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

        List<UnitDataSO> rerollUnits = SummonTableUtil.RerollShop(StageManager.Instance.ShopLevel, rerollIndices.Count);

        for (int i = 0; i < rerollIndices.Count; i++)
        {
            int index = rerollIndices[i];
            curSlots[index].SetSlot(rerollUnits[i]);
        }
    }

    public bool CheckRerollCost()
    {
        if (StageManager.Instance.SoulStone >= StageManager.Instance.RerollCost)
        {
            RerollButtonEnable();
            return true;
        }
        else
        {
            RerollButtonDisable();
            return false;
        }
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

    public void UpdateRerollCostText(int cost)
    {
        rerollCostText.text = (cost).ToString();
    }

    public void UpdateShopLevelUpCostText()
    {
        if (SummonTableUtil.CanLevelUp(StageManager.Instance.NextShopLevel))
        {
            shopLevelUpCostText.text = SummonTableUtil.GetSummonTable(StageManager.Instance.NextShopLevel).cost.ToString();
        }
    }

    public void UpdateSoulStoneText(int soulStone)
    {
        soulStoneCountText.text = soulStone.ToString();
    }

    public void UpdateShopLevelText(int shopLevel)
    {
        shopLevelText.text = shopLevel.ToString();
    }
}
