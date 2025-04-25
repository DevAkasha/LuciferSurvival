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
    protected override float Health
    {
        get => Model.Health.Value;
        set => Model.Health.SetValue(value);
    }
    public override float MoveSpeed
    {
        get => Model.MoveSpeed.Value;
        set => Model.MoveSpeed.SetValue(value);
    }
    #endregion


}

