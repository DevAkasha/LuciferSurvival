using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    protected virtual void Awake()
    {
        SetupModels();
        OnInit();
    }

    protected abstract void SetupModels();
    protected virtual void OnInit() { }
}
