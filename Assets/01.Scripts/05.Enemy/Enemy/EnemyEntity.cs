using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : MobileEntity<EnemyModel>
{
    [SerializeField] private string rcode;
    protected override float CurHealth
    {
        get => Model.Health.Value;
        set => Model.Health.SetValue(value, this);
    }
    protected override void SetupModel()
    {
        if (rcode.Equals(string.Empty))
        {
            return;
        }

        Model = new EnemyModel(DataManager.Instance.GetData<EnemyDataSO>(rcode));
    }
}
