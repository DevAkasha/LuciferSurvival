using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : BaseModel
{
    public string displayName;
    public string description;
    public int idx;

    public RxModFloat Atk;
    public RxModFloat MoveSpeed;
    public RxModFloat Health;
    public RxModFloat Range;
    public RxModFloat AtkSpeed;
    public RxModFloat AtkRecovery;

    public EnemyModel(EnemyDataSO enemyDataSO)
    {
        displayName = enemyDataSO.displayName;
        displayName = enemyDataSO.displayName;
        idx = enemyDataSO.idx;

        Atk = new(enemyDataSO.atk, nameof(Atk), this);
        MoveSpeed = new(enemyDataSO.moveSpeed, nameof(MoveSpeed), this);
        Health = new(enemyDataSO.health, nameof(Health), this);
        Range = new(enemyDataSO.atkRange, nameof(Range), this);
        AtkSpeed = new(enemyDataSO.atkSpeed, nameof(AtkSpeed), this);
        AtkRecovery = new(enemyDataSO.atkRecovery, nameof(AtkRecovery), this);
    }
    public override IEnumerable<IModifiable> GetModifiables()
    {
        yield return Atk;
        yield return MoveSpeed;
        yield return Health;
        yield return Range;
        yield return AtkSpeed;
        yield return AtkRecovery;
    }
}
