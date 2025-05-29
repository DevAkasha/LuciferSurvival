using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private ObjectPoolBase AudioPrefab;

    public void SetDayBgm()
    {
        if (AudioPrefab != null)
            PoolManager.Instance.Release(AudioPrefab);

        AudioPrefab = PoolManager.Instance.SpawnAudioSource<ObjectPoolBase>("DayBgm");
    }

    public void SetNightBgm()
    {
        if (AudioPrefab != null)
            PoolManager.Instance.Release(AudioPrefab);

        AudioPrefab = PoolManager.Instance.SpawnAudioSource<ObjectPoolBase>("NightBgm");
    }
}
