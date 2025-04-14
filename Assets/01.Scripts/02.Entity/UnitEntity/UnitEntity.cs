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


    private void Start()
    {
        
    }

    protected override void SetupModel()
    {
        if(rcode.Equals(string.Empty))
        {
            return;
        }

        //unitModel = new UnitModel(DataManager.instance.GetData<UnitDataSO>(rcode));
    }
}
