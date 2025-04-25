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

    public EnemyModel(EnemyDataSO data)
    {
        displayName = data.displayName;
        displayName = data.displayName;
        idx = data.idx;

        Atk = new(data.atk, nameof(Atk), this);
        MoveSpeed = new(data.moveSpeed, nameof(MoveSpeed), this);
        Health = new(data.health, nameof(Health), this);
        Range = new(data.atkRange, nameof(Range), this);
    }
    public override IEnumerable<IModifiable> GetModifiables()
    {
        yield return Atk;
        yield return MoveSpeed;
        yield return Health;
        yield return Range;
    }
}
