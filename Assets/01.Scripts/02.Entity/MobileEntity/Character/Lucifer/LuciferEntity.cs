using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuciferEntity : PlayerEntity
{
    protected override void SetupModel()
    {
        Model = new();
    }

    #region proxy property
    public override float Hp
    {
        get => Model.Hp.Value;
        set => Model.Hp.SetValue(value);
    }
    public override float MoveSpeed
    {
        get => Model.MoveSpeed.Value;
        set => Model.MoveSpeed.SetValue(value);
    }
    #endregion



}

