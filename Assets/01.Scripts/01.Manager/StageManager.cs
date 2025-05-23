using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    protected override bool IsPersistent => false;

    public StageModel thisStage;
    public int waveRound;

    public RxVar<int> soulStone = new RxVar<int>(0); //게임 내 재화(초기값 : 0)

    private void Start()
    {
        SetStage(GameManager.Instance.stageNumber);
        ChangeToNight();
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
    public void SetStage(int stageIndex)
    {
        if (0 > stageIndex || stageIndex > GameManager.Instance.allStageList.Count)
            return;

        waveRound = 0;
        thisStage = GameManager.Instance.allStageList[stageIndex];
        Debug.Log($"스테이지{stageIndex + 1} 준비");
    }

    public void ChangeToNight()// 밤으로 전환, 다음 라운드로 Data 변경
    {
        if (thisStage == null)
            return;

        if (waveRound < 0 || waveRound > thisStage.StageData.Count)
            waveRound = 0;

        WaveManager.Instance.SetWave(thisStage.StageData[waveRound]);
        TimeManager.Instance.SetNight();
    }

    public void ChangeToDay()// 낮으로 전환. 웨이브 시작
    {
        Debug.Log($"{waveRound + 1}웨이브 시작");
        TimeManager.Instance.SetDay();
        WaveManager.Instance.GenerateWave();

        waveRound++;
    }

    public void OnWaveEnd()
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
}
#if UNITY_EDITOR 
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