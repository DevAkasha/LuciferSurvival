using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using UnityEngine.Events;
using Ironcow.Resource;
using Ironcow.ObjectPool;
using System.Runtime.CompilerServices;

namespace Ironcow.Sound
{
#if !USE_OBJECT_POOL
    [RequireComponent(typeof(AudioSource))]
#endif
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioSource effect;
        [SerializeField] private Dictionary<string, AudioClip> audioPool = new Dictionary<string, AudioClip>();

        [HideInInspector] public bool isInit;

        private void OnValidate()
        {
#if !USE_OBJECT_POOL
#if !USE_AUTO_CACHING
            source = GetComponent<AudioSource>();
#endif
            if(effect == null)
            {
                effect = gameObject.AddComponent<AudioSource>();
            }
#else
            var audioSources = GetComponents<AudioSource>();
            foreach (var audioSource in audioSources)
            {
                DestroyImmediate(audioSource);
            }
#endif
        }

        public void Init()
        {
            isInit = true;
        }

        public async void PlayBgm(string key, bool isLoop = true)
        {
            if (!audioPool.ContainsKey(key))
                audioPool.Add(key, ResourceManager.instance.LoadAsset<AudioClip>(key, ResourceType.Audio));
#if USE_OBJECT_POOL
            var source = PoolManager.instance.SpawnAudioSource<AudioSourcePoolData>();
            source.PlayBgm(audioPool[key], isLoop);
#else
            source.clip = audioPool[key];
            source.loop = isLoop;
            source.Play();
#endif
        }

        public async void PlayOneShot(string key)
        {
            if (!audioPool.ContainsKey(key))
                audioPool.Add(key, ResourceManager.instance.LoadAsset<AudioClip>(key, ResourceType.Audio));
#if USE_OBJECT_POOL
            var source = PoolManager.instance.SpawnAudioSource<AudioSourcePoolData>();
            source.PlayOneShot(audioPool[key]);
#else
            effect.PlayOneShot(audioPool[key]);
#endif
        }

        public void StopBgm()
        {
            source.Stop();
        }

        public void SetBgmVolume(float volume)
        {
            source.volume = volume;
        }

        public void SetEffectVolume(float volume)
        {
            effect.volume = volume;
        }
    }
}