using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController: BaseController<UnitEntity,UnitModel>
{
    private void Update()
    {
        Entity.FindEnemy();
    }
}
