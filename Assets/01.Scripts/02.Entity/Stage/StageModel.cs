using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageModel : BaseModel
{
    public List<WaveModel> StageData = new();
    public List<string> StageEnemyType = new();

    public string Reward1;
    public int Reward1Count;

    public StageModel(StageDataSO stageDataSO)
    {
        StageEnemyType.AddRange(new[] {stageDataSO.enemyType1, stageDataSO.enemyType2, stageDataSO.enemyType3 });

        Reward1 = stageDataSO.reward1;
        Reward1Count = stageDataSO.reward1Count;

        string[] waveKeys = new[]
         {
            stageDataSO.wave1,
            stageDataSO.wave2,
            stageDataSO.wave3,
            stageDataSO.wave4,
            stageDataSO.wave5
        };

        foreach (var waveKey in waveKeys)
        {
            if (string.IsNullOrEmpty(waveKey))
                continue;

            // Resources 폴더 아래 "Wave" 폴더에 해당 이름의 SO가 있다고 가정
            WaveDataSO waveSO = DataManager.Instance.GetData<WaveDataSO>(waveKey);
            if (waveSO == null)
            {
                Debug.LogWarning($"WaveDataSO를 찾을 수 없습니다: {waveKey}");
                continue;
            }

            WaveModel waveModel = new WaveModel(waveSO);
            StageData.Add(waveModel);
        }
    }
}
