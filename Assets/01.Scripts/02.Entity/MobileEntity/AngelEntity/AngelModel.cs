using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AngelModel : BaseModel
{
    public int idx;

    public RxModFloat Atk;
    public RxModFloat MoveSpeed;
    public RxModFloat MaxHealth;
    public RxModFloat CurHealth;
    public RxModFloat Range;

    public RxVar<float> NormalizedHP;

    public RxStateFlagSet<PlayerStateFlag> Flags;
    public FSM<PlayerState> State;

    public AngelModel(EnemyDataSO enemyDataSO)
    {
        idx = enemyDataSO.idx;

        Atk = new(enemyDataSO.atk, nameof(Atk), this);
        MoveSpeed = new(enemyDataSO.moveSpeed, nameof(MoveSpeed), this);
        Range = new(enemyDataSO.atkRange, nameof(Range), this);

        MaxHealth = new(enemyDataSO.health, nameof(MaxHealth), this);
        CurHealth = new(MaxHealth.Value, nameof(CurHealth), this);
        NormalizedHP = new(1f, this);
        
        Action<float> recalc = _ => NormalizedHP.SetValue(CurHealth.Value / MaxHealth.Value);
        CurHealth.AddListener(recalc);
        MaxHealth.AddListener(recalc);

        Flags = new RxStateFlagSet<PlayerStateFlag>(this);

        State = new FSM<PlayerState>(PlayerState.Idle,this)
            .SetPriority(PlayerState.Death, 100)
            .SetPriority(PlayerState.Stun, 90)
            .SetPriority(PlayerState.Roll, 80)
            .SetPriority(PlayerState.Attack, 70)
            .SetPriority(PlayerState.Cast, 60)
            .SetPriority(PlayerState.Move, 20)
            .SetPriority(PlayerState.Idle, 0)
            .DriveByFlags(Flags, EvaluateState);
    }
    private PlayerState EvaluateState(RxStateFlagSet<PlayerStateFlag> flags)
    {
        if (flags.GetValue(PlayerStateFlag.Death))
            return PlayerState.Death;

        if (flags.GetValue(PlayerStateFlag.Stun) || flags.GetValue(PlayerStateFlag.Knockback))
            return PlayerState.Stun;

        if (flags.GetValue(PlayerStateFlag.Roll))
            return PlayerState.Roll;

        if (flags.GetValue(PlayerStateFlag.Attack))
            return PlayerState.Attack;

        if (flags.GetValue(PlayerStateFlag.Move))
            return PlayerState.Move;

        if (flags.GetValue(PlayerStateFlag.Cast))
            return PlayerState.Cast;

        return PlayerState.Idle; // 플래그가 모두 해제된 경우 대기 상태
    }

    public override IEnumerable<IModifiable> GetModifiables()
    {
        yield return Atk;
        yield return MoveSpeed;
        yield return CurHealth;
        yield return Range;
        yield return MaxHealth;
    }
}
