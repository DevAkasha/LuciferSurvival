using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuciferEntity : CharacterEntity
{
    public CharacterModel model;
    
    #region proxy property
    public override float Hp 
    { 
        get => model.Hp.Value; 
        set => model.Hp.SetValue(value);
    }
    public override float MoveSpeed 
    {
        get => model.MoveSpeed.Value;
        set => model.MoveSpeed.SetValue(value);
    }
    #endregion

    protected override void SetupModels()
    {
        model = new();
    }



}
