using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    //유닛 인벤토리
    public StackableUnitModel[] unitSlots = new StackableUnitModel[8];
    //유닛 장착 슬롯
    public UnitModel[] equippedUnits = new UnitModel[6];

    public SummonUnitUI summonUnitUI;

    private RxVar<int> soulStone = new RxVar<int>(0);           //게임 내 재화(초기값 : 0)
    private RxVar<int> rerollCost = new RxVar<int>(3);          //상점 리롤 비용(초기값 : 3)
    private RxVar<int> shopLevel = new RxVar<int>(1);           //상점 레벨(초기값 : 1)

    protected override void Awake()
    {
        base.Awake();
        ClearAllSlots();

        //테스트용 코드
        unitSlots[0] = new StackableUnitModel(new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0014")));
        unitSlots[1] = new StackableUnitModel(new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0025")));
        unitSlots[2] = new StackableUnitModel(new UnitModel(DataManager.Instance.GetData<UnitDataSO>("UNIT0034")));
        unitSlots[0].count = 3;
        unitSlots[1].count = 3;
        unitSlots[2].count = 3;
        SoulStone = 9;
    }

    public int SoulStone 
    {
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

    public int NextShopLevel
    {
        get { return shopLevel.Value + 1; }
    }

    //초기화
    public void Init()
    {
        //반응형 이벤트 구독
        soulStone.AddListener(v => summonUnitUI.UpdateSoulStoneText(v));
        rerollCost.AddListener(v => summonUnitUI.UpdateRerollCostText(v));
        shopLevel.AddListener(v => summonUnitUI.UpdateShopLevelUpCostText());
        shopLevel.AddListener(v => summonUnitUI.UpdateShopLevelText(v));

        summonUnitUI.UpdateShopLevelUpCostText();
        summonUnitUI.UpdateRerollCostText(RerollCost);

        //슬롯 UI 초기화
        StageUIManager.Instance.RefreshAllUnitSlots();
        StageUIManager.Instance.RefreshAllEquipSlots();
    }

    public void OnPopupClose()
    {
        soulStone.ClearRelation();
        rerollCost.ClearRelation();
        shopLevel.ClearRelation();
        shopLevel.ClearRelation();
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
            StackableUnitModel item = unitSlots[i];
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
                unitSlots[i] = new StackableUnitModel(unit);
                return;
            }
        }
    }

    //유닛 들어갈 수 있는지 체크하는 함수
    public bool CanAddUnit(UnitModel model)
    {
        for (int i = 0; i < unitSlots.Length; i++)
        {
            StackableUnitModel slot = unitSlots[i];
            if (slot != null && slot.unitModel.rcode == model.rcode && slot.count < 3)
            {
                return true;
            }
        }

        for (int i = 0; i < unitSlots.Length; i++)
        {
            if (unitSlots[i] == null)
            {
                return true;
            }
        }

        return false;
    }

    //해당 슬롯 유닛 제거
    public void RemoveUnit(int slotIndex)
    {   
        //슬롯 범위 체크(예외처리)
        if (slotIndex < 0 || slotIndex >= unitSlots.Length)
        {
            return;
        }

        StackableUnitModel item = unitSlots[slotIndex];

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

    public bool UnequipUnit(int equipIndex)
    {
        if (equipIndex < 0 || equipIndex >= equippedUnits.Length)
            return false;

        UnitModel unit = equippedUnits[equipIndex];
        if (unit == null)
            return false;

        AddUnit(unit);

        PlayerManager.Instance.Player.RemoveUnitTransform(equipIndex);

        equippedUnits[equipIndex] = null;

        StageUIManager.Instance.RefreshEquipSlot(equipIndex);
        StageUIManager.Instance.RefreshAllUnitSlots();

        return true;
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
            StackableUnitModel item = unitSlots[i];
            if (item == null)
            {
                Debug.Log($"슬롯 {i}: (비어있음)");
                continue;
            }

            Debug.Log($"슬롯 {i}: {item.unitModel.displayName}({item.count}개)");
        }
    }

    public bool CheckUnit(string rcode, int count = 1)
    {
        int findCount = 0;

        for (int i = 0; i < unitSlots.Length; i++)
        {
            if (unitSlots[i] != null && unitSlots[i].unitModel.rcode.Equals(rcode))
            {
                findCount++;
            }

            if (findCount >= count)
            {
                return true;
            }
        }

        for (int i = 0; i < equippedUnits.Length; i++)
        {
            if (equippedUnits[i] != null && equippedUnits[i].rcode.Equals(rcode))
            {
                findCount++;
            }

            if (findCount >= count)
            {
                return true;
            }
        }
        return false;
    }

    public int CalculateCompletionRate(UnionTableSO table)
    {
        //Linq를 사용하여 unit4의 값이 없으면 unit4를 제거
        List<string> codes = new List<string> { table.unit1, table.unit2, table.unit3, table.unit4 }
                              .Where(code => !string.IsNullOrEmpty(code))
                              .ToList();

        int totalGrade = 0;
        int ownedGrade = 0;

        foreach (string code in codes)
        {
            UnitDataSO data = DataManager.Instance.GetData<UnitDataSO>(code);
            int grade = (int)data.grade;  // 예: UnitDataSO.grade
            totalGrade += grade;

            if (CheckUnit(code))
                ownedGrade += grade;
        }

        if (totalGrade <= 0f)
            return 0;

        float rawRate = (float)ownedGrade / totalGrade * 100f;
        return Mathf.RoundToInt(rawRate);
    }

    // 현재 인벤토리에서 존재하는 유닛의 수를 확인하는 함수
    public int GetUnitCount(string rcode)
    {
        int total = 0;
        foreach (StackableUnitModel slot in unitSlots)
        {
            if (slot != null && slot.unitModel.rcode == rcode)
                total += slot.count;
        }

        foreach (UnitModel slot in equippedUnits)
        {
            if (slot != null && slot.rcode == rcode)
                total++;
        }
        return total;
    }

    // 인벤토리에서 rcode 유닛을 count만큼 소비
    private bool ConsumeUnit(string rcode, int count)
    {
        int remaining = count;
        for (int i = 0; i < unitSlots.Length && remaining > 0; i++)
        {
            var slot = unitSlots[i];
            if (slot != null && slot.unitModel.rcode == rcode)
            {
                int remove = Mathf.Min(slot.count, remaining);
                for (int k = 0; k < remove; k++)
                    RemoveUnit(i);
                remaining -= remove;
            }
        }
        return remaining == 0;
    }

    //유닛을 조합하기 전 인벤토리 공간 체크
    private bool CheckSlot(string rcode)
    {
        
        foreach (var slot in unitSlots)
        {
            if (slot != null && slot.unitModel.rcode == rcode)
                return true;
        }
        
        foreach (var slot in unitSlots)
        {
            if (slot == null || slot.unitModel == null)
                return true;
        }
        
        return false;
    }

    // 조합 실행 함수
    public bool CombineUnit(UnionTableSO tableSO)
    {
        if (!CheckSlot(tableSO.unitRcode))
            return false;

        //뽑으려는 유닛 정보
        UnitDataSO unitData = DataManager.Instance.GetData<UnitDataSO>(tableSO.unitRcode);
        List<string> reqs = new List<string>();
        if (!string.IsNullOrEmpty(tableSO.unit1)) reqs.Add(tableSO.unit1);
        if (!string.IsNullOrEmpty(tableSO.unit2)) reqs.Add(tableSO.unit2);
        if (!string.IsNullOrEmpty(tableSO.unit3)) reqs.Add(tableSO.unit3);
        if (!string.IsNullOrEmpty(tableSO.unit4)) reqs.Add(tableSO.unit4);

        foreach (var rcode in reqs)
        {
            if (GetUnitCount(rcode) < 1)
                return false;
        }

        if (!UseSoulStone(unitData.cost))
        {
            return false;
        }

        foreach (var rcode in reqs)
        {
            ConsumeUnit(rcode, 1);
        }

        AddUnit(new UnitModel(unitData));

        return true;
    }
}
