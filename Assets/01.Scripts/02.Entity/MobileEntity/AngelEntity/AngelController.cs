using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class AngelController : MobileController<AngelEntity, AngelModel>
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool waitAttackTime = false;
    public bool WaitAttackTime { get; set; }
    [SerializeField] public bool attackTime;

    private PlayerController player;
    private CancellationTokenSource behaviorCts;
    private BehaviorTree behaviorTree;

    private bool IsDontAct => Entity.Model.Flags.AnyActive(PlayerStateFlag.Fall, PlayerStateFlag.Knockback, PlayerStateFlag.Stun, PlayerStateFlag.Death);
    private bool IsCastable => false;   //todo.캐스트 가능성 판단 추가해야 함
    private bool IsConfuse => Entity.Model.Flags.GetValue(PlayerStateFlag.Confuse);
    private bool IsAttack
    {
        get => Entity.Model.Flags.GetValue(PlayerStateFlag.Attack);
        set => Entity.Model.Flags.SetValue(PlayerStateFlag.Attack, value);
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

        Entity.Model.State.OnEnter(PlayerState.Move, () => animator.Play("Move"));
        Entity.Model.State.OnEnter(PlayerState.Idle, () => animator.Play("Idle"));
        Entity.Model.State.OnEnter(PlayerState.Death, () => animator.Play("Death"));
        Entity.Model.State.OnEnter(PlayerState.Stun, () => animator.Play("Stun"));
        Entity.Model.State.OnEnter(PlayerState.Roll, () => animator.Play("Roll"));
        Entity.Model.State.OnEnter(PlayerState.Attack, () => animator.Play("Attack"));
        Entity.Model.State.OnEnter(PlayerState.Cast, () => animator.Play("Cast"));

        StageManager.Instance.Regist(this);

        // BT 초기화 - 한 번만 구성
        InitializeBehaviorTree();

        behaviorCts = new CancellationTokenSource();
        RunBehaviorLoop(behaviorCts.Token).Forget();
    }

    private void InitializeBehaviorTree()
    {
        // AngelEntity에 대한 BT 구성
        behaviorTree = new BehaviorTree(
            new Selector(
                // 1. 행동 불가 상태 체크
                new Sequence(
                    new Condition(() => IsDontAct),
                    new StayStillAction(Entity)
                ),
                // 2. 혼란 상태 체크
                new Sequence(
                    new Condition(() => IsConfuse && player != null),
                    new BehaviorAction(() =>
                    {
                        WaitAttackTime = false;
                        attackTime = false;
                        IsAttack = false;

                        Entity.MoveTo(player.transform.position);
                        return NodeStatus.Success;
                    })
                ),
                // 3. 공격 대기 상태 체크
                new Sequence(
                    new Condition(() => WaitAttackTime),
                    new BehaviorAction(() =>
                    {
                        if (player == null) // 여기서 체크
                        {
                            WaitAttackTime = false;
                            return NodeStatus.Failure;
                        }

                        if (attackTime)
                        {
                            if (IsInRange(Entity.Model.Range.Value))
                                Entity.OnAttack(player);

                            UniTaskVoid uniTaskVoid = Entity.StartSkillCooldown();
                            WaitAttackTime = false;
                            attackTime = false;
                            IsAttack = false;
                            return NodeStatus.Success;
                        }
                        return NodeStatus.Running;
                    })
                ),

                // 4. 캐스팅 가능 상태 체크
                new Sequence(
                    new Condition(() => IsCastable),
                    new SetFlagAction<PlayerStateFlag>(Entity.Model.Flags, PlayerStateFlag.Cast, true),
                    new StayStillAction(Entity)
                ),

                // 5. 공격 범위 내 체크
                new Sequence(
                    new Condition(() => player != null),
                    new IsEnemyInRangeCondition(Entity, player?.transform, Entity.Model.Range.Value),
                    new BehaviorAction(() =>
                    {
                        if (Entity.AttackCoolTime)
                        {
                            IsAttack = true;

                            if (Entity.Model.EnemyType == EnemyType.dasher)
                            {
                                transform.LookAt(player.transform);
                                UniTaskVoid uniTaskVoid = Entity.DeshTo(player.transform.position);
                                WaitAttackTime = true;
                                OnAttackAnimEvent(GetClipLength("Attack") + 1.5f);
                                return NodeStatus.Success;
                            }

                            transform.LookAt(player.transform);
                            Entity.StopMove();
                            WaitAttackTime = true;
                            OnAttackAnimEvent(GetClipLength("Attack"));
                            return NodeStatus.Success;
                        }
                        return NodeStatus.Running;
                    })
                ),

                // 6. 플레이어를 향해 이동
                new Sequence(
                    new Condition(() => player != null),
                    new MoveToPlayerAction(Entity, player?.transform)
                ),

                // 7. 기본 상태
                new StayStillAction(Entity)
            )
        );
    }
    public void Deinit() => AtDeinit();

    protected override void AtDeinit()
    {
        base.AtDeinit();
        WaitAttackTime = false;
        attackTime = false;
        IsAttack = false;
        player = null;

        behaviorCts?.Cancel();
        behaviorCts = null;

        behaviorTree = null;
    }

    private async UniTaskVoid RunBehaviorLoop(CancellationToken token)
    {
        try
        {
            // 매 프레임 실행 대신 시간 간격으로 실행
            float updateInterval = 0.1f; // 100ms마다 업데이트

            while (!token.IsCancellationRequested)
            {
                if (this == null || !this.isActiveAndEnabled)
                    break;

                if (behaviorTree == null)
                    break;
             
                // 캐싱된 BT 실행
                behaviorTree.Update();
            
                // 시간 간격으로 실행하여 CPU 사용량 감소
                await UniTask.Delay(TimeSpan.FromSeconds(updateInterval), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
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
        try
        {
            // behaviorCts가 null이거나 이미 취소되었으면 바로 리턴
            if (behaviorCts == null || behaviorCts.IsCancellationRequested)
                return;

            await UniTask.Delay(
                TimeSpan.FromSeconds(animaitionTime),
                DelayType.DeltaTime,
                PlayerLoopTiming.Update,
                behaviorCts.Token);

            if (player != null && behaviorCts != null && !behaviorCts.IsCancellationRequested)
            {
                attackTime = true;
            }
        }
        catch (OperationCanceledException)
        {
            // 취소는 정상 상황 - 아무것도 하지 않음
        }
        catch (System.Exception ex)
        {
            // 다른 예외는 로그만 출력
            Debug.LogWarning($"OnAttackAnimEvent 예외: {ex.Message}");
        }
    }

    public void LongRangeAttack()
    {
        var bullet = PoolManager.Instance.Spawn<ObjectPoolBase>("AttackBody", transform.position);
        var projectile = bullet as Projectile;
        
        if (projectile != null)
        {
            projectile.Init(3f, 1, Entity.Model.Atk.Value, player.transform);
        }
    }

    public float GetClipLength(string clipName)
    {
        if (animator?.runtimeAnimatorController == null) return 0f;

        // RuntimeAnimatorController의 animationClips 배열에서 직접 검색
        var clips = animator.runtimeAnimatorController.animationClips;

        foreach (var clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
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