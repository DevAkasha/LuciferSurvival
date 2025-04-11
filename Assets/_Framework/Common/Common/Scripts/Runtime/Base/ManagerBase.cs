using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class ManagerBase<T> : MonoSingleton<T>, IManagerInit where T : ManagerBase<T>
{
    public bool isInit { get; set; }

    public virtual void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
    {
        
    }
}