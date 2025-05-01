using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    //유닛 인벤토리
    public UnitInventory[] unitSlots = new UnitInventory[8];
    //유닛 장착 슬롯
    public UnitModel[] equippedUnits = new UnitModel[6];

    public SummonUnitUI summonUnitUI;

    private RxVar<int> soulStone = new RxVar<int>(0);           //게임 내 재화(초기값 : 0)
    private RxVar<int> rerollCost = new RxVar<int>(3);           //상점 리롤 비용(초기값 : 3)
    private RxVar<int> shopLevel = new RxVar<int>(1);           //상점 레벨(초기값 : 1)

    protected override void Awake()
    {
        base.Awake();
        ClearAllSlots();

        //테스트용 코드
        unitSlots[0] = new UnitInventory(new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0001")));
        unitSlots[1] = new UnitInventory(new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0002")));
        unitSlots[2] = new UnitInventory(new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0003")));
        unitSlots[0].count = 3;
        unitSlots[1].count = 3;
        unitSlots[2].count = 3;
        SoulStone = 100;
    }

    public int SoulStone {
        get { return soulStone.Value; }
        set { soulStone.SetValue(value, this); }
    }

    public int RerollCost
    {
        get { return rerollCost.Value; }
        set { rerollCost.SetValue(value, this); }
    }

    public int ShopLevel
    {
        get { return shopLevel.Value; }
        set { shopLevel.SetValue(value, this); }
    }

    //초기화
    public void Init()
    {
        //반응형 이벤트 구독
        soulStone.AddListener(v => summonUnitUI.UpdateSoulStoneText(v));
        rerollCost.AddListener(v => summonUnitUI.UpdateRerollCostText(v));
        shopLevel.AddListener(v => summonUnitUI.UpdateShopLevelUpCostText(v));
        shopLevel.AddListener(v => summonUnitUI.UpdateShopLevelText(v));

        //슬롯 UI 초기화
        StageUIManager.Instance.RefreshAllUnitSlots();
        StageUIManager.Instance.RefreshAllEquipSlots();
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

    //유닛 인벤토리 초기화
    public void InitializeInventory(UnitModel[] initialUnits)
    {
        ClearAllSlots();

        foreach (UnitModel unit in initialUnits)
        {
            AddUnit(unit);
        }
    }

    //인벤토리에 유닛 추가
    public void AddUnit(UnitModel unit)
    {
        for (int i = 0; i < unitSlots.Length; i++)
        {
            UnitInventory item = unitSlots[i];
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

    //해당 슬롯 유닛 제거
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

    //장착 유닛 모델값 출력
    public UnitModel GetEquippedUnit(int index)
    {
        if (index < 0 || index >= equippedUnits.Length) return null;
        return equippedUnits[index];
    }

    //모든 슬롯 초기화
    public void ClearAllSlots()
    {
        for (int i = 0; i < unitSlots.Length; i++)
        {
            unitSlots[i] = null;
        }
    }

    //테스트용 유닛 출력 장치
    public void PrintInventory()
    {
        for (int i = 0; i < unitSlots.Length; i++)
        {
            UnitInventory item = unitSlots[i];
            if (item == null)
            {
                Debug.Log($"슬롯 {i}: (비어있음)");
                continue;
            }

            Debug.Log($"슬롯 {i}: {item.unitModel.displayName}({item.count}개)");
        }
    }
}
