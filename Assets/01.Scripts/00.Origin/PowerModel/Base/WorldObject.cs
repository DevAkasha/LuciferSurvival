using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldObject : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        SetupModel();
        OnInit();
    }

    protected abstract void SetupModel();

    protected virtual void OnInit() { }

}
