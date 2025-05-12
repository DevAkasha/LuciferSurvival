using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

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

                // 2. 공격 대기 상태 체크
                new Sequence(
                    new Condition(() => WaitAttackTime),
                    new BehaviorAction(() => {
                        if (attackTime)
                        {
                            if (IsInRange(Entity.Model.Range.Value))
                                Entity.OnAttack(player);
                            WaitAttackTime = false;
                            attackTime = false;
                            IsAttack = false;
                            return NodeStatus.Success;
                        }
                        return NodeStatus.Running;
                    })
                ),

                // 3. 캐스팅 가능 상태 체크
                new Sequence(
                    new Condition(() => IsCastable),
                    new SetFlagAction<PlayerStateFlag>(Entity.Model.Flags, PlayerStateFlag.Cast, true),
                    new StayStillAction(Entity)
                ),

                // 4. 공격 범위 내 체크
                new Sequence(
                    new IsEnemyInRangeCondition(Entity, player?.transform, Entity.Model.Range.Value),
                    new BehaviorAction(() => {
                        IsAttack = true;
                        transform.LookAt(player.transform);
                        Entity.StopMove();
                        WaitAttackTime = true;
                        OnAttackAnimEvent();
                        return NodeStatus.Success;
                    })
                ),

                // 5. 플레이어를 향해 이동
                new Sequence(
                    new Condition(() => player != null),
                    new MoveToPlayerAction(Entity, player?.transform)
                ),

                // 6. 기본 상태
                new StayStillAction(Entity)
            )
        );
    }

    private void Update()
    {
        Entity.TakeDamaged(1f);
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
            // 매 프레임 실행 대신 시간 간격으로 실행
            float updateInterval = 0.1f; // 100ms마다 업데이트

            while (!token.IsCancellationRequested)
            {
                if (this == null || !this.isActiveAndEnabled)
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

    public async void OnAttackAnimEvent()
    {
        //애니메이션 이벤트 대신 0.5초 대기
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.DeltaTime, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        attackTime = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3.0f); // 공격 범위 디버그
    }
}