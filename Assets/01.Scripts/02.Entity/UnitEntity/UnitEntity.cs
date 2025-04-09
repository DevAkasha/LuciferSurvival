using System.Collections;
using System.Collections.Generic;
using Ironcow;
using Ironcow.BT;
using UnityEngine;

public class UnitEntity : BaseEntity
{
    [SerializeField]
    private string rcode;

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private BTRunner unitBTRunner;

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

    public void Fire(Transform target)
    { 
        Instantiate(bullet, target);
    }

    public void Attack(Transform target)
    {

    }

    public eNodeState FindEnemy()
    {
        return eNodeState.failure;
    }
}
