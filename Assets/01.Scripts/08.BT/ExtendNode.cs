// 근처에 적이 있는지 확인하는 조건 노드
using System;
using UnityEngine;

public class IsEnemyInRangeCondition : ConditionNode
{
    private readonly AngelEntity entity;
    private readonly Transform playerTransform;
    private readonly float range;

    public IsEnemyInRangeCondition(AngelEntity entity, Transform playerTransform, float range)
    {
        this.entity = entity;
        this.playerTransform = playerTransform;
        this.range = range;
    }

    public override NodeStatus Execute()
    {
        if (playerTransform == null)
            return NodeStatus.Failure;

        float sqrRange = range * range;
        float sqrDistance = (entity.transform.position - playerTransform.position).sqrMagnitude;

        return sqrDistance <= sqrRange ? NodeStatus.Success : NodeStatus.Failure;
    }
}

// 플레이어를 향해 이동하는 액션 노드
public class MoveToPlayerAction : ActionNode
{
    private readonly AngelEntity entity;
    private readonly Transform playerTransform;

    public MoveToPlayerAction(AngelEntity entity, Transform playerTransform)
    {
        this.entity = entity;
        this.playerTransform = playerTransform;
    }

    public override NodeStatus Execute()
    {
        if (playerTransform == null)
            return NodeStatus.Failure;

        entity.MoveTo(playerTransform.position);
        return NodeStatus.Success;
    }
}

// 플레이어를 공격하는 액션 노드
public class AttackPlayerAction : ActionNode
{
    private readonly AngelEntity entity;
    private readonly PlayerController player;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    public AttackPlayerAction(AngelEntity entity, PlayerController player)
    {
        this.entity = entity;
        this.player = player;
    }

    public override NodeStatus Execute()
    {
        if (player == null)
            return NodeStatus.Failure;

        if (!isAttacking)
        {
            // 공격 시작
            entity.StopMove();
            isAttacking = true;
            attackTimer = 0f;
            return NodeStatus.Running;
        }

        // 공격 진행 중
        attackTimer += Time.deltaTime;

        // 애니메이션 시간 (0.5초)이 지나면 공격 완료
        if (attackTimer >= 0.5f)
        {
            entity.OnAttack(player);
            isAttacking = false;
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }
}

// 제자리에 서 있는 액션 노드
public class StayStillAction : ActionNode
{
    private readonly AngelEntity entity;

    public StayStillAction(AngelEntity entity)
    {
        this.entity = entity;
    }

    public override NodeStatus Execute()
    {
        entity.StopMove();
        return NodeStatus.Success;
    }
}

public static class FSMBehaviorExtensions
{
    // FSM 상태 전환을 위한 BT 노드
    public static IBehaviorNode CreateStateTransitionNode<TState>(
        FSM<TState> fsm,
        TState targetState,
        IBehaviorNode condition) where TState : Enum
    {
        return new Sequence(
            condition,
            new BehaviorAction(() => {
                fsm.Request(targetState);
                return NodeStatus.Success;
            })
        );
    }
}