using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    [SerializeField] private int killCount;

    public WaveModel WaveData;

    private void Start()
    {
        PoolManager.Instance.Init(ResourceType.Enemy);
        PoolManager.Instance.Init(ResourceType.Projectile);
        GameManager.Instance.ExhangeToNight();
    }
    public void SetWave(WaveModel waveData)
    {
        WaveData = waveData;
        
        killCount = 0;
    }

    public void WaveGenerate()
    {
        if (WaveData == null)
            return;

        for (int i = 0; i < WaveData.EnemyData.Count; i++)
        {
            SpawnLoop(WaveData.EnemyData[i], WaveData.EnemySec[i], WaveData.EnemyCount[i]).Forget();
        }

    }

    private async UniTaskVoid SpawnLoop(string rcode, float delay, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (rcode == null)
                return;

            await UniTask.Delay(TimeSpan.FromSeconds(delay), DelayType.DeltaTime, PlayerLoopTiming.Update);
            SpawnManager.Instance.EnemySpawn(rcode);
        }
    }

    public void KillCountCheck()
    {
        killCount++;

        if(killCount >= EnemyCount())
        {
            Debug.Log("웨이브 종료");
            GameManager.Instance.WaveTheEnd();
        }
    }

    public int EnemyCount()
    {
        int CountArray = 0;

        for (int i = 0; i < WaveData.EnemyCount.Count; i++)
        {
            if (WaveData.EnemyCount[i] > 0)
            { 
                CountArray += WaveData.EnemyCount[i]; 
            }
        }

        return CountArray;
    }

}
