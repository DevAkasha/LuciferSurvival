
using System;
using UnityEngine.Events;


public interface IManagerInit
{
    public bool isInit { get; set; }
    public void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null);
}