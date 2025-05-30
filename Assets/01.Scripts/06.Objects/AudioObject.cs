using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : ObjectPoolBase
{
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Init(params object[] param)
    {
        throw new System.NotImplementedException();
    }
}
