using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : Singleton<GameManager> //매니저 구현체를 관리하는 코드 추가
{
    public RxVar<int> essenceOfRuin = new RxVar<int>(0);
    public List<StageModel> allStageList = new();// GameStageList 모든 스테이지 정보
    public int stageNumber;

    public int EssenceOfRuin
    {
        get => essenceOfRuin.Value;
        set => essenceOfRuin.SetValue(value, this);
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        for (int i = 1; i < 8; i++)
        {
            StageDataSO StageData = DataManager.Instance.GetData<StageDataSO>($"STG000{i}");

            if (StageData == null)
            {
                continue;
            }
            allStageList.Add(new StageModel(StageData));
        }
    }

    public void AddEssence(int essence)
    {
        EssenceOfRuin += essence;
    }

    public bool ReduceEssence(int essence)
    {
        if (EssenceOfRuin - essence < 0) return false;
            EssenceOfRuin -= essence;
        return true;
    }

    public async void PauseGame(float delayTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delayTime), DelayType.DeltaTime, PlayerLoopTiming.Update);
        Time.timeScale = 0f;
        Debug.Log("일시 정지");
    }

    public void PauseReleaseGame()
    {
        Time.timeScale = 1f;
        Debug.Log("일시 정지 해제");
    }
}

