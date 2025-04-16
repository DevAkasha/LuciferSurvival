using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : BaseModel
{
    public string rcode;
    public string displayName;
    public string description;
    public int idx;
    public float atk;
    public float moveSpeed;
    public float health;
    public float range;

    public EnemyModel(EnemyDataSO enemySO)
    {
        rcode = enemySO.rcode;
        displayName = enemySO.displayName;
        displayName = enemySO.displayName;
        idx = enemySO.idx;
        atk = enemySO.atk;
        moveSpeed = enemySO.moveSpeed;
        health = enemySO.health;
        range = enemySO.range;
    }
}
