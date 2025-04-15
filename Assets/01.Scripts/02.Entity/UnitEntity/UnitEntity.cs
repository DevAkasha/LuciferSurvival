using System;
using UnityEngine;

public class UnitEntity : BaseEntity<UnitModel>
{
    [SerializeField]
    private string rcode;

    [SerializeField]
    private GameObject bullet;

    private Collider[] colliders = new Collider[10];

    private void Start()
    {
        
    }

    protected override void SetupModel()
    {
        if(rcode.Equals(string.Empty))
        {
            return;
        }

        Model = new UnitModel(DataManager.instance.GetData<UnitDataSO>(rcode));
    }

    public void FindEnemy()
    {
        if (Physics.OverlapSphereNonAlloc(transform.position, Model.range, colliders, 1 << LayerMask.NameToLayer("Enemy")) > 0)
        {
            Model.unitState = eUnitState.Attack;
        }
        else
        {
            Array.Clear(colliders, 0, colliders.Length);
        }
    }

    public void AttackEnemy()
    {
        //가장 가까운 적을 찾아 공격하는 로직 구현
        //공격이 끝나면 Stay 상태 변환 필요
        if (colliders.Length > 0)
        {
            //공격 후 collider를 비워 새로운 적을 찾게 만듦
            //공격이 끝난 후 Stay값으로 변경하여 공격이 끝났음을 보여주는 처리
            Array.Clear(colliders, 0, colliders.Length);

        }
        Model.unitState = eUnitState.Stay;
    }
}
