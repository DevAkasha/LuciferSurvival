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

    [SerializeField] private int killCount;

    public WaveModel curWave;

    private void Start()
    {
        PoolManager.Instance.Init(ResourceType.Enemy, true);
        PoolManager.Instance.Init(ResourceType.Projectile);
        StageManager.Instance.ChangeToNight();
    }

    public void SetWave(WaveModel waveData)
    {
        curWave = waveData;
        
        killCount = 0;
    }

    public void GenerateWave()
    {
        if (curWave == null)
            return;

        for (int i = 0; i < curWave.EnemyData.Count; i++)
        {
            StartSpawnLoop(curWave.EnemyData[i], curWave.EnemySec[i], curWave.EnemyCount[i]).Forget();
        }

    }

    private async UniTaskVoid StartSpawnLoop(string rcode, float delay, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (rcode == null)
                return;

            await UniTask.Delay(TimeSpan.FromSeconds(delay), DelayType.DeltaTime, PlayerLoopTiming.Update);
            SpawnManager.Instance.EnemySpawn(rcode);

            if (GetEnemyTypes(rcode) == EnemyTypes.Boss)
                StageUIManager.Instance.OnBossWarning();
        }
    }

    public void CheckKillCount()
    {
        killCount++;

        if(killCount >= CalculateEnemyCount())
        {
            Debug.Log("웨이브 종료");
            StageManager.Instance.OnWaveEnd();
        }
    }

    private int CalculateEnemyCount()
    {
        int enemyCount = 0;

        for (int i = 0; i < curWave.EnemyCount.Count; i++)
        {
            if (curWave.EnemyCount[i] > 0)
            { 
                enemyCount += curWave.EnemyCount[i]; 
            }
        }

        return enemyCount;
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
}
