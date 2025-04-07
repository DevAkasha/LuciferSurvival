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

#if USE_AUTO_CACHING
        public override void Release()
#else
        public void Release()
#endif
        {
            PoolManager.instance.Release(this);
        }
    }

}