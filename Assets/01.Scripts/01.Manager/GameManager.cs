using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<string> gameWave;
    public StageDataSO StageData;
    public int WaveRound;

    public void WaveDataSet(string getRcode)
    {
        StageData = DataManager.Instance.GetData<StageDataSO>(getRcode);
        gameWave.AddRange(new[] { StageData.wave1, StageData.wave2, StageData.wave3, StageData.wave4, StageData.wave5 });
    }

    public void ExhangeToDay()
    {
        TimeManager.Instance.SetDay();
        WaveManager.Instance.WaveGenerate();

        WaveRound++;
    }

    public void ExhangeToNight()
    {
        if (gameWave == null)
            return;
        if (WaveRound < 0 || WaveRound > gameWave.Count)
            WaveRound = 0;

        TimeManager.Instance.SetNight();
        WaveManager.Instance.SetWave(gameWave[WaveRound]);
    }

    public void WaveTheEnd()
    {
        if(WaveRound < gameWave.Count)
        {
            ExhangeToNight();
        }
        else
        {
            //
        }
    }
}
