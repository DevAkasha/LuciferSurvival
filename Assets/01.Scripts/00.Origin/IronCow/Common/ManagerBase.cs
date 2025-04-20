using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public interface IManagerInit
{
    public bool IsInit { get; set; }
    public void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null);
}

public abstract class ManagerBase<T> : Singleton<T>, IManagerInit where T : ManagerBase<T>
{
    public bool IsInit { get; set; }

    public virtual void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
    {
        
    }
}