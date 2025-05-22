using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public List<StageModel> allStageList = new(); //@sm GameStageList 모든 스테이지 정보
    public StageModel thisStage;//@sm
    public int stageNumber;//@sm
    public int waveRound;//@sm

    public RxVar<int> soulStone = new RxVar<int>(0); //@SM           //게임 내 재화(초기값 : 0)

    private void Start()
    {
        Init();
        SetStage(0);//테스트 용
    }

    protected override void Awake()
    {
        base.Awake();
        SoulStone = 9; //테스트 코드인가요?
    }

    public int SoulStone 
    {
        get { return soulStone.Value; }
        set { soulStone.SetValue(value, this); }
    }

    public bool ReduceSoulStone(int amount)
    {
        if (soulStone.Value >= amount)
        {
            SoulStone -= amount;
            return true;
        }
        return false;
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
}
#if UNITY_EDITOR //@sm 
[CustomEditor(typeof(StageManager))]
public class StageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("전투 시작"))
            {
                StageManager.Instance.ChangeToDay();
            }

        }
    }
}
#endif