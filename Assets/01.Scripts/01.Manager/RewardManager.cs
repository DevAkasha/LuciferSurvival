using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
    protected override bool IsPersistent => false;

    // SO를 리스트로
    [SerializeField] private List<FarmingDataSO> farmingDataList;

    private Dictionary<string, FarmingDataSO> farmingDataDict;    // rcode를 빠르게 조회하기 위한 딕셔너리
    private List<(string rCode, float probability)> probabilityTable;   // 누적 확률 저장 리스트

    private new void Awake()
    {
        farmingDataDict = new Dictionary<string, FarmingDataSO>();
        probabilityTable = new List<(string, float)>();

        float total = 0f;

        foreach (var data in farmingDataList)
        {
            if (data == null || string.IsNullOrEmpty(data.rcode))
            {
                Debug.LogError("FarmingDataSO에 null이나 rCode가 비어 있음!");
                continue;
            }

            farmingDataDict[data.rcode] = data;
            total += data.probability;
            probabilityTable.Add((data.rcode, total)); // 누적 확률 저장
        }

        if (total < 100f)
            Debug.LogWarning("총 확률이 100 미만입니다.");
    }

    public void TryGatherResource()
    {
        var result = GetRandomFarmingResult();
        Debug.Log($"결과: {result.displayName}, 보상: {result.gain}");

        GiveReward(result.gain);
    }

    private FarmingDataSO GetRandomFarmingResult()
    {
        float rand = UnityEngine.Random.Range(0f, 100f);

        foreach (var (rCode, cumulativeProb) in probabilityTable)
        {
            if (rand <= cumulativeProb)
            {
                return farmingDataDict[rCode];
            }
        }

        // fallback
        var lastCode = probabilityTable.Last().rCode;
        return farmingDataDict[lastCode];
    }

    private void GiveReward(int amount)
    {
        // 보상 지급 처리
    }
}
