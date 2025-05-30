using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public List<AudioClip> BgmList;
    private AudioSource bgmSource;

    private void Start()
    {
        bgmSource = GetComponent<AudioSource>();
    }

    public void SetBgm(string bgmName)
    {
        foreach (var bgm in BgmList)
        {
            if(bgm.name == bgmName)
            {
                bgmSource.clip = bgm;
                break;
            }
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public float GetBGMVolume() => bgmSource.volume;

    public void SetEffectAudio(string effectName)
    {
        var effectAudio = PoolManager.Instance.SpawnAudioSource<AudioObject>(effectName);
    }
}
