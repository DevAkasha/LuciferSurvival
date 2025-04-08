using Ironcow.Data;
using Ironcow.ObjectPool;
using System;
using UnityEngine;

namespace Ironcow.WorldObjectBase
{
    [System.Serializable]
    public abstract class WorldBase<T> :
#if USE_OBJECT_POOL
        ObjectPoolBase
#else
#if USE_AUTO_CACHING
        MonoAutoCaching
#else
        MonoBehaviour
#endif
#endif
        where T : BaseDataSO
    {
        [HideInInspector] public T data;

        public
#if USE_OBJECT_POOL
            override 
#else
            virtual
#endif
            void Init(params object[] param)
        {

        }
#if USE_OBJECT_POOL
        public abstract void Init(T data);
#endif
    }
}