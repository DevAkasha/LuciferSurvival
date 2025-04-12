using System.Collections;
using System.Collections.Generic;
using Ironcow;
using Ironcow.BT;
using UnityEngine;

public class UnitEntity : BaseEntity<UnitModel>
{
    [SerializeField]
    private string rcode;

    [SerializeField]
    private GameObject bullet;

    private UnitModel unitModel;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        
    }

    protected override void SetupModels()
    {
        if(rcode.Equals(string.Empty))
        {
            return;
        }

        //unitModel = new UnitModel(DataManager.instance.GetData<UnitDataSO>(rcode));
    }
}
