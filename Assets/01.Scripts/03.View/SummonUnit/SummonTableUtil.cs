using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SummonTableUtil
{
    private static int summonCount = 5;

    private static Dictionary<int, List<UnitDataSO>> GetUnitDict()
    {
        List<UnitDataSO> unitList = DataManager.instance.GetDatas<UnitDataSO>();
        Dictionary<int, List<UnitDataSO>> unitDict = new Dictionary<int, List<UnitDataSO>>();

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
        List<SummonTableSO> summonTableList = DataManager.instance.GetDatas<SummonTableSO>();
        Dictionary<int, SummonTableSO> summonTableDict = new Dictionary<int, SummonTableSO>();

        for(int i = 0; i < summonTableList.Count; i++)
        {
            summonTableDict.Add(summonTableList[i].level, summonTableList[i]);
        }

        return summonTableDict;
    }

    public static List<UnitDataSO> RerollShop(int shopLevel)
    {
        List<UnitDataSO> shopUnits = new List<UnitDataSO>();

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
}
