using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : ObjectPoolBase
{
    public AudioSource audioSource;

    public override void Init(params object[] param)
    {
        //음향 크기 조절 추가 필요
        StartCoroutine(Release());
    }

    IEnumerator Release()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        PoolManager.Instance.Release(this);
    }
}
