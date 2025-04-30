using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public UnitInventory[] unitSlots = new UnitInventory[8];
    [SerializeField]
    SummonUnitUI summonUnitUI;

    private RxVar<int> soulStone = new RxVar<int>();
    private RxVar<int> rerollCost = new RxVar<int>(3);
    private RxVar<int> shopLevel = new RxVar<int>(1);

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < unitSlots.Length; i++)
        {
            unitSlots[i] = null;
        }

        //soulStone.AddListener(v => summonUnitUI.UpdateSoulStoneText(v));
        //rerollCost.AddListener(v => summonUnitUI.UpdateRerollCostText(v));
        //shopLevel.AddListener(v => summonUnitUI.UpdateShopLevelUpCostText(v));
        //shopLevel.AddListener(v => summonUnitUI.UpdateShopLevelText(v));

        //unitSlots[0] = new UnitInventory(new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0001")));
        //unitSlots[1] = new UnitInventory(new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0002")));
        //unitSlots[2] = new UnitInventory(new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0003")));

        //unitSlots[0].count = 3;
        //unitSlots[1].count = 3;
        //unitSlots[2].count = 3;

        StageUIManager.Instance.RefreshAllUnitSlots();

        SoulStone = 100;
    }

    public int SoulStone { 
        get { return soulStone.Value; }
        set { soulStone.SetValue(value); }
    }

    public int RerollCost
    {
        get { return rerollCost.Value; }
        set { rerollCost.SetValue(value); }
    }

    public int ShopLevel
    {
        get { return shopLevel.Value; }
        set { shopLevel.SetValue(value); }
    }

    public bool UseSoulStone(int cost)
    {
        if (soulStone.Value >= cost)
        {
            SoulStone -= cost;
            return true;
        }
        return false;
    }

    public void GetSoulStone(int count)
    {
        SoulStone += count;
    }

    public void InitializeInventory(UnitModel[] initialUnits)
    {
        ClearAllSlots();

        foreach (var unit in initialUnits)
        {
            AddUnit(unit);
        }
    }

    public void AddUnit(UnitModel unit)
    {
        for (int i = 0; i < unitSlots.Length; i++)
        {
            var item = unitSlots[i];
            if (item != null && item.unitModel.rcode == unit.rcode && item.count < 3)
            {
                item.AddCount();
                return;
            }
        }

        for (int i = 0; i < unitSlots.Length; i++)
        {
            if (unitSlots[i] == null)
            {
                unitSlots[i] = new UnitInventory(unit);
                return;
            }
        }
    }

    public void RemoveUnit(int slotIndex)
    {   
        //슬롯 범위 체크(예외처리)
        if (slotIndex < 0 || slotIndex >= unitSlots.Length)
        {
            return;
        }

        UnitInventory item = unitSlots[slotIndex];

        if (item == null)
        {
            return;
        }

        item.RemoveCount();

        if (item.count <= 0)
        {
            unitSlots[slotIndex] = null;
        }

        StageUIManager.Instance.RefreshUnitSlot(slotIndex);
    }

    public void ClearAllSlots()
    {
        for (int i = 0; i < unitSlots.Length; i++)
        {
            unitSlots[i] = null;
        }
    }

    public void PrintInventory()
    {
        for (int i = 0; i < unitSlots.Length; i++)
        {
            var item = unitSlots[i];
            if (item == null)
            {
                Debug.Log($"슬롯 {i}: (비어있음)");
                continue;
            }

            Debug.Log($"슬롯 {i}: {item.unitModel.displayName}({item.count}개)");
        }
    }
}
