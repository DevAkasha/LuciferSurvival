using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public List<AudioClip> BgmList;
    [SerializeField] private AudioSource bgmSource;

    private void Start()
    {
        InitializeAudioData();
    }

    public void SetBgm(string bgmName)
    {
        foreach (var bgm in BgmList)
        {
            if (bgm.name == bgmName)
            {
                bgmSource.clip = bgm;
                break;
            }
        }

        bgmSource.Play();
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

    public void InitializeAudioData()
    {
        BgmList.Clear();

        // Resources/resourceType 폴더에서 모든 프리팹을 불러오기
        var audioClip = Resources.LoadAll<AudioClip>(ResourceType.Audio);

        foreach (var bgm in audioClip)
        {
            BgmList.Add(bgm);
        }
    }
}
