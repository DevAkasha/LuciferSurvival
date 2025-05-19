using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveModel : BaseModel
{
    public int idx;
    
    public int Stage;
    public int NightTime;

    public List<string> EnemyData = new();
    public List<int> EnemyCount = new();
    public List<float> EnemySec = new();

    public WaveModel(WaveDataSO waveData)
    {
        idx = waveData.idx;

        Stage = waveData.Stage;
        NightTime = waveData.nightTime;

        EnemyData.AddRange(new[] { waveData.enemy1rcode, waveData.enemy2rcode, waveData.enemy3rcode, waveData.enemy4rcode, waveData.bossrcode });
        EnemyCount.AddRange(new[] { waveData.enemy1Count, waveData.enemy2Count, waveData.enemy3Count, waveData.enemy4Count, waveData.bossCount });
        EnemySec.AddRange(new[] { waveData.enemy1Sec, waveData.enemy2Sec, waveData.enemy3Sec, waveData.enemy4Sec, waveData.bossDelaySec });
    }
}
