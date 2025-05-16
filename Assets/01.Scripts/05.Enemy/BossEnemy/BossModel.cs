using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Idle,
    Move,
    Stun,
    Roll,
    Skill1,
    Skill2,
    Skill3,
    Attack,
    Cast,
    Death
}
public class BossModel : BaseModel
{
    public int idx;

    public RxModFloat Atk;
    public RxModFloat MoveSpeed;
    public RxModFloat MaxHealth;
    public RxModFloat CurHealth;
    public RxModFloat AtkRange;
    public RxModFloat AtkSpeed;
    public RxModFloat Skill1Range;
    public RxModFloat Skill2Range;
    public RxModFloat Skill3Range;
    public RxModFloat Skill1CT;
    public RxModFloat Skill2CT;
    public RxModFloat Skill3CT;

    public RxVar<float> NormalizedHP;
    public EnemyType atkType = EnemyType.boss;
    public RxStateFlagSet<PlayerStateFlag> Flags;
    public FSM<BossState> State;

    public BossModel(BossDataSO bossDataSO)
    {
        idx = bossDataSO.idx;

        Atk = new(bossDataSO.atk, nameof(Atk), this);
        MoveSpeed = new(bossDataSO.moveSpeed, nameof(MoveSpeed), this);
        AtkSpeed = new(bossDataSO.atkSpeed, nameof(AtkSpeed), this);
        AtkRange = new(bossDataSO.atkRange, nameof(AtkRange), this);
        Skill1Range = new(bossDataSO.skill1Range, nameof(Skill1Range), this);
        Skill2Range = new(bossDataSO.skill2Range, nameof(Skill2Range), this);
        Skill3Range = new(bossDataSO.skill3Range, nameof(Skill3Range), this);
        Skill1CT = new(bossDataSO.skill1CT, nameof(Skill1CT), this);
        Skill2CT = new(bossDataSO.skill2CT, nameof(Skill2CT), this);
        Skill3CT = new(bossDataSO.skill3CT, nameof(Skill3CT), this);

        MaxHealth = new(bossDataSO.health, nameof(MaxHealth), this);
        CurHealth = new(MaxHealth.Value, nameof(CurHealth), this);
        NormalizedHP = new(1f, this);
        //atkType = bossDataSO.atkType; 추가해야함

        Action<float> recalc = _ => NormalizedHP.SetValue(CurHealth.Value / MaxHealth.Value, this);
        CurHealth.AddListener(recalc);
        MaxHealth.AddListener(recalc);

        Flags = new RxStateFlagSet<PlayerStateFlag>(this);

        State = new FSM<BossState>(BossState.Idle, this)
            .SetPriority(BossState.Death, 100)
            .SetPriority(BossState.Stun, 90)
            .SetPriority(BossState.Roll, 80)
            .SetPriority(BossState.Skill1, 70)
            .SetPriority(BossState.Skill2, 60)
            .SetPriority(BossState.Skill3, 50)
            .SetPriority(BossState.Attack, 40)
            .SetPriority(BossState.Cast, 30)
            .SetPriority(BossState.Move, 20)
            .SetPriority(BossState.Idle, 0)
            .DriveByFlags(Flags, EvaluateState);
    }
    private BossState EvaluateState(RxStateFlagSet<PlayerStateFlag> flags)
    {
        if (flags.GetValue(PlayerStateFlag.Death))
            return BossState.Death;

        if (flags.GetValue(PlayerStateFlag.Stun) || flags.GetValue(PlayerStateFlag.Knockback))
            return BossState.Stun;

        if (flags.GetValue(PlayerStateFlag.Roll))
            return BossState.Roll;

        if (flags.GetValue(PlayerStateFlag.Skill1))
            return BossState.Skill1;

        if (flags.GetValue(PlayerStateFlag.Skill2))
            return BossState.Skill2;

        if (flags.GetValue(PlayerStateFlag.Skill3))
            return BossState.Skill3;

        if (flags.GetValue(PlayerStateFlag.Attack))
            return BossState.Attack;

        if (flags.GetValue(PlayerStateFlag.Move))
            return BossState.Move;

        if (flags.GetValue(PlayerStateFlag.Cast))
            return BossState.Cast;

        return BossState.Idle; // 플래그가 모두 해제된 경우 대기 상태
    }

    public override IEnumerable<IModifiable> GetModifiables()
    {
        yield return Atk;
        yield return MoveSpeed;
        yield return CurHealth;
        yield return MaxHealth;
        yield return AtkRange;
        yield return AtkSpeed;
        yield return Skill1Range;
        yield return Skill2Range;
        yield return Skill3Range;
        yield return Skill1CT;
        yield return Skill2CT;
        yield return Skill3CT;
    }
}
