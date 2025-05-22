using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public List<StageModel> allStageList = new(); //@sm GameStageList 모든 스테이지 정보
    public StageModel thisStage;//@sm
    public int stageNumber;//@sm
    public int waveRound;//@sm

    public RxVar<int> essenceOfRuin;

    public int EssenceOfRuin
    {
        get => essenceOfRuin.Value;
        set => essenceOfRuin.SetValue(value, this);
    }

    private void Start()
    {
        Init();
        SetStage(0);//테스트 용
    }

    public void SetStage(int stageIndex)//@sm
    {
        if (0 > stageIndex || stageIndex > allStageList.Count)
            return;

        waveRound = 0;
        thisStage = allStageList[stageIndex];
        Debug.Log($"스테이지{stageIndex + 1} 준비");
    }

    public void ChangeToNight()//@sm 밤으로 전환, 다음 라운드로 Data 변경
    {
        if (thisStage == null)
            return;

        if (waveRound < 0 || waveRound > thisStage.StageData.Count)
            waveRound = 0;

        WaveManager.Instance.SetWave(thisStage.StageData[waveRound]);
        TimeManager.Instance.SetNight();
    }

    public void ChangeToDay()//@sm 낮으로 전환. 웨이브 시작
    {
        Debug.Log($"{waveRound + 1}웨이브 시작");
        TimeManager.Instance.SetDay();
        WaveManager.Instance.GenerateWave();

        waveRound++;
    }

    public void OnWaveEnd()//@sm
    {
        if (waveRound < thisStage.StageData.Count)
        {
            ChangeToNight();
            Debug.Log("다음 웨이브 준비");
        }
        else
        {
            StageUIManager.Instance.OnStageCleatWindow();
            Debug.Log("스테이지 클리어!");
        }
    }

    public void Init() //@sm
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

#if UNITY_EDITOR //@sm 
[CustomEditor(typeof(GameManager))] 
public class GameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("전투 시작"))
            {
                GameManager.Instance.ChangeToDay();
            }

        }
    }
}
#endif
