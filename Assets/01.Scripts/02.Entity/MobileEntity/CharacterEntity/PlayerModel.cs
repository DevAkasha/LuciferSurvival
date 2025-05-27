using System;
using System.Collections.Generic;

public enum PlayerState
{
    Idle,
    Move,
    Stun,
    Roll,
    Attack,
    Cast,
    Death
}

public enum PlayerStateFlag
{
    Knockback,
    Stun,
    Fall,
    Death,
    Move,
    Roll,       //플레이어 전용
    Attack,
    Skill1,     //보스 전용
    Skill2,
    Skill3,
    Cast,
    Slow,       // FSM 대상 아님
    Confuse     // FSM 대상 아님
}

public class PlayerModel: BaseModel
{
    public RxModFloat MaxHealth;
    public RxModFloat CurHealth;
    public RxModFloat MoveSpeed;

    public readonly RxStateFlagSet<PlayerStateFlag> Flags;
    public readonly FSM<PlayerState> State;

    public RxVar<float> NormalizedHP;

    public PlayerModel()
    {
        MaxHealth = new(250f, nameof(MaxHealth), this);
        CurHealth = new(MaxHealth.Value, nameof(CurHealth), this);
        MoveSpeed = new(4f, nameof(MoveSpeed), this);
        NormalizedHP = new(1f, this);
        
        Action<float> recalc = _ => NormalizedHP.SetValue(CurHealth.Value / MaxHealth.Value, this);
        CurHealth.AddListener(recalc);
        MaxHealth.AddListener(recalc);

        Flags = new RxStateFlagSet<PlayerStateFlag>(this);

        State = new FSM<PlayerState>(PlayerState.Idle,this)
            .SetPriority(PlayerState.Death, 100)
            .SetPriority(PlayerState.Stun, 90)
            .SetPriority(PlayerState.Roll, 80)
            .SetPriority(PlayerState.Attack, 70)
            .SetPriority(PlayerState.Move, 20)
            .SetPriority(PlayerState.Cast, 10)
            .SetPriority(PlayerState.Idle, 0)
            .DriveByFlags(Flags, EvaluateState);
    }

    private PlayerState EvaluateState(RxStateFlagSet<PlayerStateFlag> flags)
    {
        if (flags.GetValue(PlayerStateFlag.Death))
            return PlayerState.Death;

        if (flags.GetValue(PlayerStateFlag.Stun) || flags.GetValue(PlayerStateFlag.Knockback) || flags.GetValue(PlayerStateFlag.Fall))
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
        yield return MaxHealth;
        yield return CurHealth;
        yield return MoveSpeed;
    }
}