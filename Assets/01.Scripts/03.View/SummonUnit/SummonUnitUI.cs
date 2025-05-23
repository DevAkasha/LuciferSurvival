using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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

    

    private void OnSoulStoneChanged(int value) => UpdateSoulStoneText(value);
    private void OnRerollCostChanged(int value) => UpdateRerollCostText(value);
    private void OnShopLevelChanged(int value) => UpdateShopLevelUpCostText();
    private void OnShopLevelUpCostChanged(int value) => UpdateShopLevelUpCostText();

    private int SoulStone 
    {
        get => StageManager.Instance.SoulStone;
        set => StageManager.Instance.SoulStone = value;
    }
    private int RerollCost
    {
        get => UnitManager.Instance.RerollCost;
        set => UnitManager.Instance.RerollCost = value;
    }
    private int ShopLevel
    {
        get => UnitManager.Instance.ShopLevel;
        set => UnitManager.Instance.ShopLevel = value;
    }
    private int NextShopLevel
    {
        get => UnitManager.Instance.NextShopLevel;
    }
    //초기화
    public void InitUI()//Ui 프레젠터로 이동
    {
        //반응형 이벤트 구독 
        StageManager.Instance.soulStone.AddListener(OnSoulStoneChanged);
        UnitManager.Instance.rerollCost.AddListener(OnRerollCostChanged);
        UnitManager.Instance.shopLevel.AddListener(OnShopLevelChanged);
        UnitManager.Instance.shopLevel.AddListener(OnShopLevelUpCostChanged);

        UpdateShopLevelUpCostText();
        UpdateRerollCostText(UnitManager.Instance.RerollCost);

        //슬롯 UI 초기화
        StageUIManager.Instance.RefreshAllUnitSlots();
        StageUIManager.Instance.RefreshAllEquipSlots();
    }

    public void InitShop()
    {
        curSlots = summonSlotLayout.GetComponentsInChildren<SummonSlot>();
        SetRandomUnit();
        CheckRerollCost();
    }

    public void OnPopupClose() //Ui 프레젠터로 이동 리무브리스너형태로
    {
        StageManager.Instance.soulStone.RemoveListener(OnSoulStoneChanged);
        UnitManager.Instance.rerollCost.RemoveListener(OnRerollCostChanged);
        UnitManager.Instance.shopLevel.RemoveListener(OnShopLevelChanged);
        UnitManager.Instance.shopLevel.RemoveListener(OnShopLevelUpCostChanged);
    }

    public void OnclickShopLevelUp()
    {
        //현재 상점 레벨 체크 후 레벨업이 가능한 재화이면 레벨업
        if (SummonTableUtil.CanLevelUp(NextShopLevel))
        {
            if (StageManager.Instance.ReduceSoulStone(SummonTableUtil.GetSummonTable(NextShopLevel).cost))
            {
                ShopLevel = NextShopLevel;
            }
        }
    }

    public void OnclickRerollUnit()
    {
        if (StageManager.Instance.ReduceSoulStone(RerollCost))
        {
            SetRandomUnit(true);
        }
    }

    private void SetRandomUnit(bool isReroll = false)
    {
        List<int> rerollIndices = new();

        for (int i = 0; i < curSlots.Length; i++)
        {
            if (!curSlots[i].unitLock.isLocked)
                rerollIndices.Add(i);
        }

        List<UnitDataSO> rerollUnits = SummonTableUtil.RerollShop(ShopLevel, rerollIndices.Count, isReroll);

        for (int i = 0; i < rerollIndices.Count; i++)
        {
            int index = rerollIndices[i];
            curSlots[index].SetSlot(rerollUnits[i], i, SummonTableUtil.purchaseList[i]);
        }
    }

    public bool CheckRerollCost()
    {
        if (SoulStone >= RerollCost)
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
        if (SummonTableUtil.CanLevelUp(NextShopLevel))
        {
            shopLevelUpCostText.text = SummonTableUtil.GetSummonTable(NextShopLevel).cost.ToString();
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
