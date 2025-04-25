using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class AngelController : MobileController<AngelEntity, AngelModel>
{

    [SerializeField] private Animator animator;

    private PlayerController player;

    private bool IsDontAct => Entity.Model.Flags.AnyActive(PlayerStateFlag.Fall, PlayerStateFlag.Knockback, PlayerStateFlag.Stun, PlayerStateFlag.Attack, PlayerStateFlag.Death);
    private bool IsCastable => false;   //todo.캐스트 가능성 판단 추가해야함
    private bool IsAttack { get => Entity.Model.Flags.GetValue(PlayerStateFlag.Attack); set => Entity.Model.Flags.SetValue(PlayerStateFlag.Attack, value); }
    private bool IsCast { get => Entity.Model.Flags.GetValue(PlayerStateFlag.Cast); set => Entity.Model.Flags.SetValue(PlayerStateFlag.Cast, value); }

    protected override void OnInit()
    {
        animator = GetComponent<Animator>(); 
    }

    private void Start()
    {
        player = PlayerManager.Instance.Player;

        Entity.Model.State.OnEnter(PlayerState.Move, () => animator.Play("Move"));
        Entity.Model.State.OnEnter(PlayerState.Idle, () => animator.Play("Idle"));
        Entity.Model.State.OnEnter(PlayerState.Death, () => animator.Play("Death"));
        Entity.Model.State.OnEnter(PlayerState.Stun, () => animator.Play("Stun"));
        Entity.Model.State.OnEnter(PlayerState.Roll, () => animator.Play("Roll"));
        Entity.Model.State.OnEnter(PlayerState.Attack, () => animator.Play("Attack"));
        Entity.Model.State.OnEnter(PlayerState.Cast, () => animator.Play("Cast"));
        Entity.Model.State.OnEnter(PlayerState.Death, () => animator.Play("Death"));

        RunBehaviorLoop().Forget(); // UniTask를 무시하고 실행
    }
    private async UniTaskVoid RunBehaviorLoop()
    {
        var token = this.GetCancellationTokenOnDestroy();

        while (!token.IsCancellationRequested)
        {
            var bt = BuildBehaviorTree();
            if (bt.Check()) bt.Run();

            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
        }
    }
    private RxBehaviorNode BuildBehaviorTree()
    {
        return new Selector(new[]
        {
            Node_DontActCheck(),
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
            Debug.Log($"{name} 공격 중");
            //Todo.애니메이터 어택모션
            DOVirtual.DelayedCall(1.0f, () =>
            {
                if (IsInRange(Entity.Model.Range.Value))
                    Entity.OnAttack(player);
                IsAttack = false;
            });
        });

    public bool IsInRange(float range)
    {
        if (player == null) 
            return false;
        float sqrRange = range * range;
        return (transform.position - player.transform.position).sqrMagnitude < sqrRange;
    }

    private RxBehaviorNode Node_MoveToPlayer() =>
        ConditionAction.Create(() => player != null, () =>
        {
            Entity.MoveTo(player.transform.position);
        });

    private RxBehaviorNode Node_StandStill() =>
        ConditionAction.Create(() => true, () => { });

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3.0f); // 공격 범위 디버그
    }
}