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
    public RxVar<int> soulCore = new RxVar<int>(0); //게임 내 재화(초기값 : 0)

    public List<AngelController> angels = new(); //게임 내 재화(초기값 : 0)
    public BossController? boss;

    private void Start()
    {
        SetStage(GameManager.Instance.stageNumber);
        ChangeToNight();
        SoulStone = 1000;
        //SoulCore = 1000;
    }
    public int SoulStone
    {
        get { return soulStone.Value; }
        set { soulStone.SetValue(value, this); }
    }

    public int SoulCore
    {
        get { return soulCore.Value; }
        set { soulCore.SetValue(value, this); }
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

    public void Regist(AngelController angel) => angels.Add(angel);
    public void Regist(BossController boss) => this.boss = boss;
    public void Unregist(AngelController angel) => angels.Remove(angel);
    public void Unregist(BossController boss) => this.boss = null;

    public bool ReduceSoulCore(int amount)
    {
        if (soulCore.Value >= amount)
        {
            SoulCore -= amount;
            return true;
        }
        return false;
    }

    public void AddSoulCore(int amount)
    {
        SoulCore += amount;
        Debug.Log($"영혹핵 {amount}개 획득");
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

        AudioManager.Instance.SetBgm("NightBgm");
        WaveManager.Instance.SetWave(thisStage.StageData[waveRound]);
        TimeManager.Instance.SetNight();
    }

    public void ChangeToDay()// 낮으로 전환. 웨이브 시작
    {
        if (TimeManager.Instance.currentTimeState != TimeState.Night)
            return;

        Debug.Log($"{waveRound + 1}웨이브 시작");
        AudioManager.Instance.SetBgm("DayBgm");
        TimeManager.Instance.SetDay();
        WaveManager.Instance.GenerateWave();
    }

    public void OnWaveEnd()
    {
        if (waveRound < thisStage.StageData.Count - 1)
        {
            waveRound++;
            ChangeToNight();
            Debug.Log("다음 웨이브 준비");
        }
        else
        {
            StageUIManager.Instance.OnStageCleatWindow();
            Debug.Log("스테이지 클리어!");
        }
    }

    public void OnPlayerDeath()
    {
        StageUIManager.Instance.OnPlayerDeathWindow();
        SummonTableUtil.InitUnitList();
    }

    public void DeinitAllEnemy()
    {
        boss?.Deinit();
        if (angels.Count == 0) return;

        foreach (var angel in angels)
        {
            angel.Deinit();
        }
    }
    protected override void OnDestroy()
    {
        soulStone?.ClearRelation();
        soulCore?.ClearRelation();
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