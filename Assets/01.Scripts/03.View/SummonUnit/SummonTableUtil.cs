using System.Collections.Generic;
using UnityEngine;

public static class SummonTableUtil
{
    private static Dictionary<int, List<UnitDataSO>> unitDict = new Dictionary<int, List<UnitDataSO>>();
    private static Dictionary<int, SummonTableSO> summonTableDict = new Dictionary<int, SummonTableSO>();
    public static List<UnitDataSO> shopUnits { get; } = new List<UnitDataSO>();
    public static bool[] purchaseList = new bool[5];

    private static Dictionary<int, List<UnitDataSO>> GetUnitDict()
    {
        unitDict.Clear();
        List<UnitDataSO> unitList = DataManager.Instance.GetDatas<UnitDataSO>();

        foreach (var unit in unitList)
        {
            if (!unitDict.ContainsKey((int)unit.grade))
                unitDict[(int)unit.grade] = new List<UnitDataSO>();

            unitDict[(int)unit.grade].Add(unit);
        }

        return unitDict;
    }

    private static Dictionary<int, SummonTableSO> GetSummonTableDict()
    {
        summonTableDict.Clear();
        List<SummonTableSO> summonTableList = DataManager.Instance.GetDatas<SummonTableSO>();

        for (int i = 0; i < summonTableList.Count; i++)
        {
            summonTableDict.Add(summonTableList[i].level, summonTableList[i]);
        }

        return summonTableDict;
    }

    public static List<UnitDataSO> RerollShop(int shopLevel, int summonCount, bool isReroll = false)
    {
        if (!isReroll && shopUnits.Count != 0) 
            return shopUnits;
        shopUnits.Clear();
        System.Array.Clear(purchaseList, 0, purchaseList.Length);

        // 확률표 가져오기
        int[] tierChances = GetSummonTableDict()[shopLevel].summonRate;

        for (int i = 0; i < summonCount; i++)
        {
            int selectedTier = GetRandomTier(tierChances);
            UnitDataSO randomUnit = GetRandomUnitFromTier(selectedTier);
            shopUnits.Add(randomUnit);
        }

        return shopUnits;
    }

    private static int GetRandomTier(int[] chances)
    {
        int roll = Random.Range(0,100) + 1;
        int cumulative = 0;

        for (int i = 0; i < chances.Length; i++)
        {
            cumulative += chances[i];
            if (roll <= cumulative)
                return i + 1;
        }

        return 1;
    }

    private static UnitDataSO GetRandomUnitFromTier(int tier)
    {
        List<UnitDataSO> pool = GetUnitDict()[tier];
        int index = Random.Range(0, pool.Count);
        return pool[index];
    }

    public static void ClearAllChildren(Transform tableTransform)
    {
        foreach (Transform child in tableTransform)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static SummonTableSO GetSummonTable(int shopLevel)
    {
        SummonTableSO outValue = null;
        if (GetSummonTableDict().TryGetValue(shopLevel, out outValue))
        {
            return outValue;
        }
        return null;
    }

    public static bool CanLevelUp(int level)
    {
        if (GetSummonTable(level) != null)
        {
            return true;
        }
        return false;
    }

    public static void InitUnitList()
    {
        shopUnits.Clear();
        for(int i = 0; i < purchaseList.Length; i++)
        {
            purchaseList[i] = false;
        }
    }
}
