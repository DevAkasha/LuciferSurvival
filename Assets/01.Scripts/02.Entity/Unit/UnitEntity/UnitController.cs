using System.Collections;
using System.Collections.Generic;
using Ironcow.BT;
using UnityEngine;

public class UnitController: BaseController<UnitEntity,UnitModel>
{
    [SerializeField]
    private BTRunner unitBTRunner;
    
    private float attackDelay = 0f;
    private bool isAttack = false;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        //unitBTRunner = new BTRunner(name.Replace("(Clone)", "")).SetActions(this);
        StartCoroutine(StateLoop());
    }

    IEnumerator StateLoop()
    {
        while (true)
        {
            if (isAttack)
            {
                attackDelay += 0.1f;
            }
            UnitStateAction(Entity.Model.unitState);
            Debug.Log(attackDelay);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void UnitStateAction(eUnitState state)
    {
        if (state == eUnitState.Stay)
        {
            if (attackDelay >= Entity.Model.atkSpeed)
            {
                isAttack = false;
                attackDelay = 0f;
            }
            Entity.FindEnemy();
        }
        else if(state == eUnitState.Attack && !isAttack)
        {
            Debug.Log("공격 시작");
            Entity.AttackEnemy();
            attackDelay = 0f;
            isAttack = true;
        }
        else
        {
            Entity.Model.unitState = eUnitState.Stay;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Entity.Model.range);
    }
}
