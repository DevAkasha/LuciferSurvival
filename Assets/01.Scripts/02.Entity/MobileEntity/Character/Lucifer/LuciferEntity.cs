using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuciferEntity : PlayerEntity
{
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

    protected override void SetupModels()
    {
        Model = new();
    }

}
