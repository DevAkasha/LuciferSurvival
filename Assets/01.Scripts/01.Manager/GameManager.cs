using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private List<StageModel> gameWave = new();
    private StageModel thisStage;
    public int WaveRound;

    public int EssenceOfRuin;

    private void Start()
    {
        WaveDataInfo();
        WaveDataSet(1);//테스트 용
        PoolManager.Instance.Init(ResourceType.Enemy);
        PoolManager.Instance.Init(ResourceType.Projectile);
        ExhangeToNight();
    }

    public void WaveDataSet(int i)
    {
        thisStage = gameWave[i];
    }

    public void ExhangeToNight()//밤으로 전환, 다음 라운드로 Data 변경
    {
        if (thisStage == null)
            return;

        if (WaveRound < 0 || WaveRound > thisStage.StageData.Count)
            WaveRound = 0;

        WaveManager.Instance.SetWave(thisStage.StageData[WaveRound]);
        TimeManager.Instance.SetNight();
    }

    public void ExhangeToDay()//낮으로 전환. 웨이브 시작
    {
        Debug.Log($"{WaveRound + 1}웨이브 시작");
        TimeManager.Instance.SetDay();
        WaveManager.Instance.WaveGenerate();

        WaveRound++;
    }

    public void WaveTheEnd()
    {
        if (WaveRound < gameWave.Count)
        {
            ExhangeToNight();
            Debug.Log("다음 웨이브 준비");
        }
        else
        {
            Debug.Log("스테이지 클리어!");
        }
    }

    public void WaveDataInfo()
    {
        for (int i = 1; i < 8; i++)
        {
            StageDataSO StageData = DataManager.Instance.GetData<StageDataSO>($"STG000{i}");

            if (StageData == null)
            {
                continue;
            }

            gameWave.Add(new StageModel(StageData));
        }
    }

    public void GetEssence(int Essence)
    {
        EssenceOfRuin += Essence;
    }

    public void LessEssence(int Essence)
    {
        EssenceOfRuin -= Essence;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("일시 정지");
    }
    public void PauseReleaseGame()
    {
        Time.timeScale = 1f;
        Debug.Log("일시 정지 해제");
    }
}
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
                ((GameManager)target).ExhangeToDay();
            }

        }
    }
}

