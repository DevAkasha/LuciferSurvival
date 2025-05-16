using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    [SerializeField] private int killCount;
    [SerializeField] private string rcode;//들어간 SO 확인용

    public WaveDataSO WaveData;

    private void Start()
    {
        SetWave("WAVE0001");
    }

    public void SetWave(string getRcode)
    {
        rcode = getRcode;
        WaveData = DataManager.Instance.GetData<WaveDataSO>(rcode);
        
        killCount = 0;
    }

    public void WaveGenerate()
    {
        if (WaveData == null)
            return;

        string[] spawnRcode = { WaveData.enemy1rcode, WaveData.enemy2rcode, WaveData.enemy3rcode, WaveData.enemy4rcode, WaveData.bossrcode };
        float[] spawnDelays = { WaveData.enemy1Sec, WaveData.enemy2Sec, WaveData.enemy3Sec, WaveData.enemy4Sec, WaveData.bossDelaySec };
        int[] spawnCounts = { WaveData.enemy1Count, WaveData.enemy2Count, WaveData.enemy3Count, WaveData.enemy4Count, WaveData.bossCount };

        for (int i = 0; i < spawnDelays.Length; i++)
        {
            SpawnLoop(spawnRcode[i],spawnDelays[i], spawnCounts[i]).Forget();
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

        if(killCount > EnemyCount())
        {
            GameManager.Instance.WaveTheEnd();
        }
    }

    public int EnemyCount()
    {
        int CountArray = 0;
        int[] spawnCounts = { WaveData.enemy1Count, WaveData.enemy2Count, WaveData.enemy3Count, WaveData.enemy4Count };

        for (int i = 0; i < spawnCounts.Length; i++)
        {
            if (spawnCounts[i] > 0)
            { 
                CountArray += spawnCounts[i]; 
            }
        }

        return CountArray;
    }
}
[CustomEditor(typeof(WaveManager))]
public class WaveSeter : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("WAVE0001 초기화"))
            {
                ((WaveManager)target).SetWave("WAVE0001");
            }
            if (GUILayout.Button("WAVE0005 초기화"))
            {
                ((WaveManager)target).SetWave("WAVE0005");
            }
            if (GUILayout.Button("웨이브 시작"))
            {
                ((WaveManager)target).WaveGenerate();
            }

        }
    }
}
