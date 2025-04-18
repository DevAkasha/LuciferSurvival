using Ironcow.ObjectPool;
using System.Collections;
using UnityEngine;

public class AudioSourcePoolData : ObjectPoolBase
{
    [SerializeField] private AudioSource source;
    public bool isBgm { get; private set; }

    public override void Init(params object[] param)
    {
        
    }

    public void PlayBgm(AudioClip clip, bool isLoop)
    {
        isBgm = true;
        source.clip = clip;
        source.loop = isLoop;
        source.Play();
        if (!isLoop) StartCoroutine(OnRelease(clip.length));
    }

    public void PlayOneShot(AudioClip clip)
    {
        isBgm = false;
        source.PlayOneShot(clip);
        StartCoroutine(OnRelease(clip.length));
    }

    public IEnumerator OnRelease(float delay)
    {
        yield return new WaitForSeconds(delay);
        isBgm = false;
        PoolManager.instance.Release(this);
    }

    public void SetSource(AudioSource source)
    {
        this.source = source;
    }
}
