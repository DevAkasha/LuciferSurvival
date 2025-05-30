using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    protected override bool IsPersistent => false;
    //보유중인 유닛어레이
    public StackableUnitModel[] curUnitArray = new StackableUnitModel[8];

    //장착중인 유닛어레이
    public UnitModel[] equippedUnitArray = new UnitModel[6];

    public RxVar<int> rerollCost = new RxVar<int>(3);         //상점 리롤 비용(초기값 : 3)
    public RxVar<int> shopLevel = new RxVar<int>(1);         //상점 레벨(초기값 : 1)
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

    protected override void Awake()
    {
        base.Awake();
        ClearCurUnitArray();
    }

    //유닛어레이 초기화
    public void InitializeUnitArray(UnitModel[] initialUnits)
    {
        ClearCurUnitArray();

        foreach (UnitModel unit in initialUnits)
        {
            AddUnit(unit);
        }
    }

    //유닛어레이 유닛 추가
    public void AddUnit(UnitModel unit)
    {
        for (int i = 0; i < curUnitArray.Length; i++)
        {
            StackableUnitModel item = curUnitArray[i];
            if (item != null && item.unitModel.rcode == unit.rcode && item.count < 3)
            {
                item.AddCount();
                return;
            }
        }

        for (int i = 0; i < curUnitArray.Length; i++)
        {
            if (curUnitArray[i] == null)
            {
                curUnitArray[i] = new StackableUnitModel(unit);
                return;
            }
        }
    }

    //유닛 들어갈 수 있는지 체크하는 함수
    public bool CanAddUnit(UnitModel model)
    {
        for (int i = 0; i < curUnitArray.Length; i++)
        {
            StackableUnitModel slot = curUnitArray[i];
            if (slot != null && slot.unitModel.rcode == model.rcode && slot.count < 3)
            {
                return true;
            }
        }

        for (int i = 0; i < curUnitArray.Length; i++)
        {
            if (curUnitArray[i] == null)
            {
                return true;
            }
        }

        return false;
    }

    //인벤토리 내 해당인덱스 유닛 제거
    public void RemoveUnit(int slotIndex)
    {
        //슬롯 범위 체크(예외처리)
        if (slotIndex < 0 || slotIndex >= curUnitArray.Length)
        {
            return;
        }

        StackableUnitModel item = curUnitArray[slotIndex];

        if (item == null)
        {
            return;
        }

        item.RemoveCount();

        if (item.count <= 0)
        {
            curUnitArray[slotIndex] = null;
        }

        StageUIManager.Instance.RefreshUnitSlot(slotIndex);
    }

    //유닛 장착 인벤토리 내 해당인덱스 유닛 제거
    public void RemoveEquippedUnit(int slotIndex)
    {
        //슬롯 범위 체크(예외처리)
        if (slotIndex < 0 || slotIndex >= equippedUnitArray.Length)
        {
            return;
        }

        if (equippedUnitArray[slotIndex] == null)
        {
            return;
        }

        equippedUnitArray[slotIndex] = null;

        StageUIManager.Instance.RefreshEquipSlot(slotIndex);
    }

    //장착 유닛 모델값 출력
    public UnitModel GetEquippedUnit(int index)
    {
        if (index < 0 || index >= equippedUnitArray.Length) return null;
        return equippedUnitArray[index];
    }

    //모든 슬롯 초기화
    public void ClearCurUnitArray()
    {
        for (int i = 0; i < curUnitArray.Length; i++)
        {
            curUnitArray[i] = null;
        }
    }

    //테스트용 유닛 출력 장치
    public void PrintInventory()
    {
        for (int i = 0; i < curUnitArray.Length; i++)
        {
            StackableUnitModel item = curUnitArray[i];
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

        for (int i = 0; i < curUnitArray.Length; i++)
        {
            if (curUnitArray[i] != null && curUnitArray[i].unitModel.rcode.Equals(rcode))
            {
                findCount++;
            }

            if (findCount >= count)
            {
                return true;
            }
        }

        for (int i = 0; i < equippedUnitArray.Length; i++)
        {
            if (equippedUnitArray[i] != null && equippedUnitArray[i].rcode.Equals(rcode))
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
        foreach (StackableUnitModel slot in curUnitArray)
        {
            if (slot != null && slot.unitModel.rcode == rcode)
                total += slot.count;
        }

        foreach (UnitModel slot in equippedUnitArray)
        {
            if (slot != null && slot.rcode == rcode)
                total++;
        }
        return total;
    }

    // 인벤토리에서 rcode 유닛을 count만큼 소비
    private bool ConsumeInventory(string rcode, int count)
    {
        int remaining = count;
        for (int i = 0; i < curUnitArray.Length && remaining > 0; i++)
        {
            var slot = curUnitArray[i];
            if (slot != null && slot.unitModel.rcode == rcode)
            {
                int remove = Mathf.Min(slot.count, remaining);
                // slot.RemoveCount()를 count만큼 호출
                for (int k = 0; k < remove; k++)
                    RemoveUnit(i);
                remaining -= remove;
            }
        }
        return remaining == 0;
    }

    // 유닛 장착 인벤토리에서 rcode 유닛을 count만큼 소비
    private bool ConsumeEquipment(string rcode)
    {
        for (int i = 0; i < equippedUnitArray.Length; i++)
        {
            var slot = equippedUnitArray[i];
            if (slot != null && slot.rcode == rcode)
            {
                RemoveEquippedUnit(i);
                return true;
            }
        }
        return false;
    }

    //유닛을 조합하기 전 인벤토리 공간 체크
    private bool CheckSlot(string rcode)
    {

        foreach (var slot in curUnitArray)
        {
            if (slot != null && slot.unitModel.rcode == rcode)
                return true;
        }

        foreach (var slot in curUnitArray)
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

        if (!StageManager.Instance.ReduceSoulStone(unitData.cost))
        {
            return false;
        }

        foreach (var rcode in reqs)
        {
            if(ConsumeInventory(rcode, 1))
            {
                //인벤토리 소비시 해야될 행동
            }
            else if(ConsumeEquipment(rcode))
            {
                //유닛 장착 인벤토리 소비시 해야될 행동
            }
        }

        AddUnit(new UnitModel(unitData));

        return true;
    }


    public bool SellUnit(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= curUnitArray.Length)
        {
            return false;
        }

        StackableUnitModel item = curUnitArray[slotIndex];

        if ((int)item.unitModel.grade == 5)
        {
            return false;
        }

        StageManager.Instance.SoulStone += CalculateCost(item.unitModel.cost);

        RemoveUnit(slotIndex);

        StageUIManager.Instance.RefreshUnitSlot(slotIndex);

        return true;
    }

    //판매금액 계산
    public int CalculateCost(int cost)
    {
        return Mathf.RoundToInt(cost * 2f / 3f);
    }

    public bool UnequipUnit(int equipIndex)
    {
        if (equipIndex < 0 || equipIndex >= equippedUnitArray.Length)
            return false;

        UnitModel unit = equippedUnitArray[equipIndex];
        if (unit == null)
            return false;

        AddUnit(unit);

        PlayerManager.Instance.Player.RemoveUnitTransform(equipIndex);

        equippedUnitArray[equipIndex] = null;

        StageUIManager.Instance.RefreshEquipSlot(equipIndex);
        StageUIManager.Instance.RefreshAllUnitSlots();

        return true;
    }
}
