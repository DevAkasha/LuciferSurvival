using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldObject : ObjectPoolBase
{
    public override void Init(params object[] param) { }

    protected virtual void OnEnable()
    {
        SetupModel();
        OnInit();
    }

    protected abstract void SetupModel();

    protected virtual void OnInit() { }

}
