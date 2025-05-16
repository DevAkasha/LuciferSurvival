using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<string> gameWave;
    [SerializeField] public StageDataSO StageData;
    public int WaveRound;

    private void Start()
    {
        WaveDataSet("STG0001");//테스트 용
        PoolManager.Instance.Init(ResourceType.Enemy);
        ExhangeToNight();
    }

    public void WaveDataSet(string getRcode)
    {
        StageData = DataManager.Instance.GetData<StageDataSO>(getRcode);
        gameWave.AddRange(new[] { StageData.wave1, StageData.wave2, StageData.wave3, StageData.wave4, StageData.wave5 });
    }

    public void ExhangeToNight()//밤으로 전환, 다음 라운드로 Data 변경
    {
        if (gameWave == null)
            return;
        if (WaveRound < 0 || WaveRound > gameWave.Count)
            WaveRound = 0;

        TimeManager.Instance.SetNight();
        WaveManager.Instance.SetWave(gameWave[WaveRound]);
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
        if(WaveRound < gameWave.Count)
        {
            ExhangeToNight();
            Debug.Log("다음 웨이브 준비");
        }
        else
        {
            //
        }
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

