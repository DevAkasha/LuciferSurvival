using Ironcow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ironcow.ObjectPool
{
    public abstract class ObjectPoolBase
#if USE_AUTO_CACHING
    : MonoAutoCaching
#else
    : MonoBehaviour
#endif
    {
        public abstract void Init(params object[] param);

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

#if USE_AUTO_CACHING && UNITY_EDITOR
        public override 
#endif
        void Release()
        {
            PoolManager.instance.Release(this);
        }
    }

}