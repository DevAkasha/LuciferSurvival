using System.Collections;
using System.Collections.Generic;
using Ironcow.BT;
using UnityEngine;

public class UnitController: BaseController<UnitEntity,UnitModel>
{
    [SerializeField]
    private BTRunner unitBTRunner;

    private Collider[] colliders = new Collider[10];        //10은 적을수도 있음

    private void Start()
    {
        //Init();
    }

    public void Init()
    {
        unitBTRunner = new BTRunner(name.Replace("(Clone)", "")).SetActions(this);
        StartCoroutine(BTLoop());
    }
    IEnumerator BTLoop()
    {
        while (true)
        {
            unitBTRunner.Operate();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public eNodeState CheckEnemy()
    {
        //유닛 Model에 대한 사거리 적용 필요
        if (Physics.OverlapSphereNonAlloc(transform.position, 100, colliders, 1 << LayerMask.NameToLayer("Food")) > 0)
        {
            if (colliders.Length > 0)
            {
                return eNodeState.success;
            }
        }
        return eNodeState.failure;
    }

}
