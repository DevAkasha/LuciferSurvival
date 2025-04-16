using System.Collections;
using System.Collections.Generic;
using Ironcow.BT;
using UnityEngine;

public class UnitController: BaseController<UnitEntity,UnitModel>
{
    private float attackDelay = 0f;
    private bool isAttack = false;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
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
}
