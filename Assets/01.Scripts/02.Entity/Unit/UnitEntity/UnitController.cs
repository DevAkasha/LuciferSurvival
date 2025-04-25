using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController: BaseController<UnitEntity,UnitModel>
{
    private float attackDelay = 0f;

    private void Update()
    {
        attackDelay += Time.deltaTime;
        UnitStateAction(Entity.Model.unitState);
    }

    public void UnitStateAction(eUnitState state)
    {
        if (state == eUnitState.Stay)
        {
            Entity.FindEnemy();
        }
        else if(state == eUnitState.Attack)
        {
            if (attackDelay >= (1f / Entity.Model.atkSpeed))
            {
                Entity.AttackEnemy();
                attackDelay = 0f;
            }
            Entity.Model.unitState = eUnitState.Stay;
        }
    }
}
