using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelEntity : MobileEntity<AngelModel>
{
    [SerializeField] private string rcode;
    public override float Health 
    { 
        get => Model.Health.Value; 
        set => Model.Health.SetValue(value); 
    }

    protected override void SetupModel()
    {
        if (rcode.Equals(string.Empty))
        {
            return;
        }

        Model = new AngelModel(DataManager.Instance.GetData<EnemyDataSO>(rcode));
    }

}
