using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
    protected override bool IsPersistent => false;

    [SerializeField] private List<FarmingDataSO> farmingDataList;

    // 누적 확률 테이블, 정렬된 리스트 사용
    private List<(FarmingDataSO data, float cumulativeProbability)> probabilityTable;

    // 확률 테이블 세팅 (초기화)
    private new void Awake()
    {
        probabilityTable = new List<(FarmingDataSO, float)>();
        
        float total = 0f; 
        // 인덱스 기반으로 리스트 안에 들어있는 데이터를 순회
        for (int i = 0; i < farmingDataList.Count; i++) 
        {
            var data = farmingDataList[i]; // 현재 인덱스 i에 해당하는 데이터를 꺼냄
            if (data == null) // data가 null일 경우를 위해 인덱스 기반으로
            {
                continue; // null을 누적 확률 계산에 넣으면 오류발생으로 인해 건너뛰기
            }

            total += data.probability; // total에 지금까지 더한 확률을 저장, total의 역할
            probabilityTable.Add((data, total)); // 누적된 total과 data를 튜플로 엮어서 probablitiyTable에 추가
        }
    }

    /// <summary>
    /// 자원채집 함수
    /// </summary>
    public void TryGatherResource()
    {
        var result = GetRandomFarmingResult();
        // 추후 이쪽엔 UI부분이나 다른 내용 넣기, 현재는 확인용으로 Debug.Log 사용
        Debug.Log($"결과: {result.displayName}, 보상: {result.gain}");

        GiveReward(result.gain); // 실 보상
    }

    /// <summary>
    /// 자원 보상 랜덤보상
    /// </summary>
    /// <returns></returns>
    private FarmingDataSO GetRandomFarmingResult()
    {
        // 0부터 100까지의 랜덤한 float 값을 뽑기 (확률 비교용)
        float rand = UnityEngine.Random.Range(0f, 100f);
        int left = 0;
        int right = probabilityTable.Count - 1;

        // 이진탐색 이용, left가 right보다 작거나 같을 동안 반복하면서 중간값을 기준으로 탐색, rand가 속한 구간을 찾기
        while (left <= right)
        {
            int mid = (left + right) / 2;
            float currentProb = probabilityTable[mid].cumulativeProbability;

            // 랜덤 숫자가 누적 확률보다 작거나 같을경우
            if (rand <= currentProb)
            {
                // 재확인, 첫 항목이거나 이전 구간보다 rand가 큰 경우
                if (mid == 0 || rand > probabilityTable[mid - 1].cumulativeProbability)
                {
                    return probabilityTable[mid].data; // 반환
                }
                right = mid - 1; // 왼쪽구간 재탐색
            }
            else
            {
                left = mid + 1; // rand가 더 크므로 오른쪽구간 재탐색
            }
        }

        // 어떤 항목도 선택되지 않았을 경우 방지, 리턴
        return probabilityTable[^1].data;
    }

    /// <summary>
    /// 보상지급
    /// </summary>
    /// <param name="amount"></param>
    private void GiveReward(int amount)
    {
        int currentSoulStone = StageManager.Instance.SoulStone;
        int newAmount = currentSoulStone + amount;
        StageManager.Instance.SoulStone = Mathf.Max(0, newAmount);
    }

}
