using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public enum EnemyTypes
{
    Unknown,
    Boss,
    Enemy
}

public class WaveManager : Singleton<WaveManager>
{
    protected override bool IsPersistent => false;

    private RxVar<int> killCount = new RxVar<int>();

    public WaveModel curWave;

    public List<int> spawnCount = new(); 
    public int KillCount
    {
        get => killCount.Value;
        set => killCount.SetValue(value, this);
    }

    private void Start()
    {
        PoolManager.Instance.Init(ResourceType.Enemy, true);
        PoolManager.Instance.Init(ResourceType.Projectile);
        PoolManager.Instance.Init(ResourceType.Audio);
        StageManager.Instance.ChangeToNight();
    }

    public void SetWave(WaveModel waveData)
    {
        curWave = waveData;

        KillCount = 0;
    }

    public void GenerateWave()
    {
        if (curWave == null)
            return;
        spawnCount.Clear();
        for (int i = 0; i < curWave.EnemyData.Count; i++)
        {
            StartSpawnLoop(curWave.EnemyData[i], curWave.EnemySec[i], curWave.EnemyCount[i]).Forget();
        }

    }

    private async UniTaskVoid StartSpawnLoop(string rcode, float delay, int count)
    {
        var idx = spawnCount.Count;
        this.spawnCount.Add(count);
        for (int i = 0; i < count; i++)
        {
            if (rcode == null)
                return;

            await UniTask.Delay(TimeSpan.FromSeconds(delay), DelayType.DeltaTime, PlayerLoopTiming.Update);
            if (spawnCount.Count == 0 || spawnCount[idx] == 0) break;
            SpawnManager.Instance.EnemySpawn(rcode);

            if (GetEnemyTypes(rcode) == EnemyTypes.Boss)
                StageUIManager.Instance.OnBossWarning();
        }
    }

    public void CheckKillCount()
    {
        if(KillCount >= CalculateAllCount())
        {
            Debug.Log("웨이브 종료");
            StageManager.Instance.OnWaveEnd();
        }
    }

    public int CalculateEnemyCount()
    {
        int enemyCount = 0;

        for (int i = 0; i < curWave.EnemyCount.Count - 1; i++)
        {
            if (curWave.EnemyCount[i] > 0)
            { 
                enemyCount += curWave.EnemyCount[i]; 
            }
        }

        return enemyCount;
    }

    public int CalculateAllCount()
    {
        int allCount = 0;

        for (int i = 0; i < curWave.EnemyCount.Count; i++)
        {
            if (curWave.EnemyCount[i] > 0)
            {
                allCount += curWave.EnemyCount[i];
            }
        }

        return allCount;
    }

    public EnemyTypes GetEnemyTypes(string rcode) //필요시 static 전환하면 좋음
    {
        if (string.IsNullOrEmpty(rcode))
            return EnemyTypes.Unknown;

        if (rcode.StartsWith("BOSS"))
            return EnemyTypes.Boss;

        if (rcode.StartsWith("ENE"))
            return EnemyTypes.Enemy;

        return EnemyTypes.Unknown;
    }

    //killCount 값 구독 낮 시작시 구독 필요
    public void SetKillCountListener()
    {
        killCount.AddListener(V =>
        {
            TimeManager.Instance.SetInfoText(KillCount);
        });
    }

    //killCount 값 구독 밤 시작시 구독 해제 필요
    public void RemoveKillCountListener()
    {
        killCount.ClearRelation();
    }
}
