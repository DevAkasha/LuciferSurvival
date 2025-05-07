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

        behaviorCts = new CancellationTokenSource();
        RunBehaviorLoop(behaviorCts.Token).Forget();
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
            while (!token.IsCancellationRequested)
            {
                if (this == null || !this.isActiveAndEnabled)
                    break;

                var bt = BuildBehaviorTree();
                if (bt.Check()) bt.Run();

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

    private RxBehaviorNode BuildBehaviorTree()
    {
        return new Selector(new[]
        {
            Node_DontActCheck(),
            Node_WaitAttackCheck(),
            Node_CastableCheck(),
            Node_AttackCheck(),
            Node_MoveToPlayer(),
            Node_StandStill()
        });
    }

    private RxBehaviorNode Node_DontActCheck() =>
        ConditionAction.Create(() => IsDontAct, () =>
        { 
            Entity.StopMove(); 
        });

    private RxBehaviorNode Node_WaitAttackCheck() =>
        ConditionAction.Create(() => WaitAttackTime, () =>
        {
            if (attackTime)
            {
                if (IsInRange(Entity.Model.Range.Value))
                    Entity.OnAttack(player);
                WaitAttackTime = false;
                attackTime = false;
                IsAttack = false;
            }
        });

    private RxBehaviorNode Node_CastableCheck() =>
        ConditionAction.Create(() => IsCastable, () =>
        {
            IsCast = true;
            Entity.StopMove();
        });

    private RxBehaviorNode Node_AttackCheck() =>
        ConditionAction.Create(() => IsInRange(Entity.Model.Range.Value), () =>
        {
            IsAttack = true;
            transform.LookAt(player.transform);
            Entity.StopMove();
            WaitAttackTime = true;
            OnAttackAnimEvent(); // 애니메이션 이벤트 대신 직접호출중
        });

    private RxBehaviorNode Node_MoveToPlayer() =>
        ConditionAction.Create(() => player != null, () =>
        {
            Entity.MoveTo(player.transform.position);
        });

    private RxBehaviorNode Node_StandStill() =>
        ConditionAction.Create(() => true, () => { });

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