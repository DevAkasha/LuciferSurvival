using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyFlag { Dead, Falling }
public class AngelModel : BaseModel
{
    public int idx;

    public RxModFloat Atk;
    public RxModFloat MoveSpeed;
    public RxModFloat Health;
    public RxModFloat Range;

    public RxStateFlagSet<EnemyFlag> Flags;

    public AngelModel(EnemyDataSO enemyDataSO)
    {
        idx = enemyDataSO.idx;

        Atk = new(enemyDataSO.atk, nameof(Atk), this);
        MoveSpeed = new(enemyDataSO.moveSpeed, nameof(MoveSpeed), this);
        Health = new(enemyDataSO.health, nameof(Health), this);
        Range = new(enemyDataSO.range, nameof(Range), this);

        Flags = new RxStateFlagSet<EnemyFlag>(this);
        Flags.SetCondition(EnemyFlag.Dead, () => Health.Value <= 0f);
    }

    public override IEnumerable<IModifiable> GetModifiables()
    {
        yield return Atk;
        yield return MoveSpeed;
        yield return Health;
        yield return Range;
        
    }

}
