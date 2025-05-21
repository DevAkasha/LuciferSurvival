using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;

public class BossController : MobileController<BossEntity, BossModel>
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool waitAttackTime = false;
    public bool WaitAttackTime { get; set; }
    [SerializeField] public bool attackTime;

    private PlayerController player;
    private CancellationTokenSource behaviorCts;
    private BehaviorTree behaviorTree;
    [SerializeField] private Dictionary<string, bool> skillCooldownFlags = new();

    private bool IsDontAct => Entity.Model.Flags.AnyActive(PlayerStateFlag.Fall, PlayerStateFlag.Knockback, PlayerStateFlag.Stun, PlayerStateFlag.Death);
    private bool IsCastable => false;   //todo.캐스트 가능성 판단 추가해야 함
    private bool IsAttack
    {
        get => Entity.Model.Flags.GetValue(PlayerStateFlag.Attack);
        set => Entity.Model.Flags.SetValue(PlayerStateFlag.Attack, value);
    }

    private bool IsSkill1
    {
        get => Entity.Model.Flags.GetValue(PlayerStateFlag.Skill1);
        set => Entity.Model.Flags.SetValue(PlayerStateFlag.Skill1, value);
    }

    private bool IsSkill2
    {
        get => Entity.Model.Flags.GetValue(PlayerStateFlag.Skill2);
        set => Entity.Model.Flags.SetValue(PlayerStateFlag.Skill2, value);
    }

    private bool IsSkill3
    {
        get => Entity.Model.Flags.GetValue(PlayerStateFlag.Skill3);
        set => Entity.Model.Flags.SetValue(PlayerStateFlag.Skill3, value);
    }
    private bool IsCast
    {
        get => Entity.Model.Flags.GetValue(PlayerStateFlag.Cast);
        set => Entity.Model.Flags.SetValue(PlayerStateFlag.Cast, value);
    }

    protected override void AtInit()
    {
        animator = GetComponent<Animator>();

        player = PlayerManager.Instance.Player;
        HealthBarManager.Instance.Attach(this);
        InitializeCooldowns();

        Entity.Model.State.OnEnter(BossState.Move, () => animator.Play("Move"));
        Entity.Model.State.OnEnter(BossState.Idle, () => animator.Play("Idle"));
        Entity.Model.State.OnEnter(BossState.Death, () => animator.Play("Death"));
        Entity.Model.State.OnEnter(BossState.Stun, () => animator.Play("Stun"));
        Entity.Model.State.OnEnter(BossState.Roll, () => animator.Play("Roll"));
        Entity.Model.State.OnEnter(BossState.Attack, () => animator.Play("Attack"));
        Entity.Model.State.OnEnter(BossState.Skill1, () => animator.Play("Skill1"));
        Entity.Model.State.OnEnter(BossState.Skill2, () => animator.Play("Skill2"));
        Entity.Model.State.OnEnter(BossState.Skill3, () => animator.Play("Skill3"));
        Entity.Model.State.OnEnter(BossState.Cast, () => animator.Play("Cast"));

        // BT 초기화 - 한 번만 구성
        InitializeBehaviorTree();

        behaviorCts = new CancellationTokenSource();
        RunBehaviorLoop(behaviorCts.Token).Forget();
    }

    private void InitializeBehaviorTree()
    {
        // BossEntity에 대한 BT 구성
        behaviorTree = new BehaviorTree(
            new Selector(
                // 1. 행동 불가 상태 체크
                new Sequence(
                    new Condition(() => IsDontAct),
                    new BehaviorAction(() =>
                    {
                        Entity.StopMove();
                        return NodeStatus.Success;
                    })
                ),

                // 2. 공격 대기 상태 체크
                new Sequence(
                    new Condition(() => WaitAttackTime),
                    new BehaviorAction(() =>
                    {
                        if (attackTime)
                        {
                            if (IsInRange(Entity.Model.AtkRange.Value))
                                Entity.OnAttack(player);
                            WaitAttackTime = false;
                            attackTime = false;
                            IsAttack = false;
                            IsSkill1 = false;
                            IsSkill2 = false;
                            IsSkill3 = false;
                            return NodeStatus.Success;
                        }
                        return NodeStatus.Running;
                    })
                ),

                // 3. 캐스팅 가능 상태 체크
                new Sequence(
                    new Condition(() => IsCastable),
                    new SetFlagAction<PlayerStateFlag>(Entity.Model.Flags, PlayerStateFlag.Cast, true),
                    new BehaviorAction(() =>
                    {
                        Entity.StopMove();
                        return NodeStatus.Success;
                    })
                ),

                // 4. 스킬1 체크
                new Sequence(
                    new Condition(() => IsInRange(Entity.Model.Skill1Range.Value) && skillCooldownFlags.GetValueOrDefault("Skill1", true)),
                    new BehaviorAction(() =>
                    {
                        IsSkill1 = true;
                        transform.LookAt(player.transform);
                        Entity.StopMove();
                        WaitAttackTime = true;
                        StartSkillCooldown("Skill1", Entity.Model.Skill1CT.Value).Forget();
                        OnAttackAnimEvent(GetClipLength("Skill1"));
                        return NodeStatus.Success;
                    })
                ),

                // 5. 스킬2 체크
                new Sequence(
                    new Condition(() => IsInRange(Entity.Model.Skill2Range.Value) && skillCooldownFlags.GetValueOrDefault("Skill2", true)),
                    new BehaviorAction(() =>
                    {
                        IsSkill2 = true;
                        transform.LookAt(player.transform);
                        Entity.StopMove();
                        WaitAttackTime = true;
                        StartSkillCooldown("Skill2", Entity.Model.Skill1CT.Value).Forget();
                        OnAttackAnimEvent(GetClipLength("Skill2"));
                        return NodeStatus.Success;
                    })
                ),

                // 6. 스킬3 체크
                new Sequence(
                    new Condition(() => IsInRange(Entity.Model.Skill3Range.Value) && skillCooldownFlags.GetValueOrDefault("Skill3", true)),
                    new BehaviorAction(() =>
                    {
                        IsSkill3 = true;
                        transform.LookAt(player.transform);
                        Entity.StopMove();
                        WaitAttackTime = true;
                        StartSkillCooldown("Skill3", Entity.Model.Skill1CT.Value).Forget();
                        OnAttackAnimEvent(GetClipLength("Skill3"));
                        return NodeStatus.Success;
                    })
                ),

                // 7. 공격 범위 내 체크
                new Sequence(
                    new Condition(() => IsInRange(Entity.Model.AtkRange.Value)),
                    new BehaviorAction(() =>
                    {
                        IsAttack = true;
                        transform.LookAt(player.transform);
                        Entity.StopMove();
                        WaitAttackTime = true;
                        OnAttackAnimEvent(GetClipLength("Attack"));
                        return NodeStatus.Success;
                    })
                ),

                // 8. 플레이어를 향해 이동
                new Sequence(
                    new Condition(() => player != null),
                    new BehaviorAction(() =>
                    {
                        Entity.MoveTo(player.transform.position);
                        return NodeStatus.Success;
                    })
                ),

                // 9. 기본 상태
                new BehaviorAction(() =>
                {
                    Entity.StopMove();
                    return NodeStatus.Success;
                })
            )
        );
    }

    private void Update()
    {
        Entity.TakeDamaged(1000f);
    }

    protected override void AtDisable()
    {
        behaviorCts?.Cancel();
    }

    protected override void AtDestroy()
    {
        behaviorCts?.Cancel();
        behaviorCts = null;
    }

    private async UniTaskVoid RunBehaviorLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (this == null || !this.isActiveAndEnabled)
                    break;

                // 캐싱된 BT 실행
                behaviorTree.Update();

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException)
        {
            // 취소 처리 - 정상
        }
        catch (Exception ex)
        {
            Debug.LogError($"BT 루프 오류: {ex.Message}");
        }
    }

    public bool IsInRange(float range)
    {
        if (player == null)
            return false;
        float sqrRange = range * range;
        return (transform.position - player.transform.position).sqrMagnitude < sqrRange;
    }

    public async void OnAttackAnimEvent(float animaitionTime)
    {
        //애니메이션 이벤트 대신 AtkSpeed만큼 대기
        await UniTask.Delay(TimeSpan.FromSeconds(animaitionTime), DelayType.DeltaTime, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        attackTime = true;
    }

    private async UniTaskVoid StartSkillCooldown(string skillKey, float cooldown)
    {
        if (cooldown < 0)
            return;

        var token = this.GetCancellationTokenOnDestroy();

        skillCooldownFlags[skillKey] = false; // 쿨타임 시작
        await UniTask.Delay(TimeSpan.FromSeconds(cooldown), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
        skillCooldownFlags[skillKey] = true;  // 쿨타임 종료
    }

    private void InitializeCooldowns()
    {
        skillCooldownFlags["Skill1"] = true;
        skillCooldownFlags["Skill2"] = true;
        skillCooldownFlags["Skill3"] = true;
    }

    public float GetClipLength(string clipName)
    {
        var controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null) return 0f;

        foreach (var layer in controller.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.name == clipName)
                {
                    var clip = state.state.motion as AnimationClip;
                    return clip != null ? clip.length : 0f;
                }
            }
        }
        return 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3.0f); // 공격 범위 디버그
    }
}