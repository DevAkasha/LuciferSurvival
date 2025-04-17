using UnityEngine;
using UnityEngine.AI;

public class AngelController : MobileController<AngelEntity, AngelModel>
{
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private Animator animator;

    private Transform player;
    private bool IsDead => Entity.Model.Flags.GetValue(EnemyFlag.Dead);
    private bool IsFalling => Entity.Model.Flags.GetValue(EnemyFlag.Falling);

    protected override void OnInit()
    {
        navMesh = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        UnityTimer.ScheduleRepeating(0.1f, () =>
        {
            var bt = BuildBehaviorTree();
            if (bt.Check())  
                bt.Run();
        });
    }
    private void Start()
    {
        player = PlayerManager.Instance.Player.transform;
        Entity.Model.Flags.AddListener(EnemyFlag.Dead, OnDeath);
    }

    private void OnDeath(bool isDead)
    {
        if (!isDead) return;
        navMesh.isStopped = true;
        animator.Play("Dead");
    }

    public void MoveTo(Vector3 target)
    {
        if (IsDead || IsFalling) return;
        rigid.velocity = Vector3.zero;
        navMesh.speed = Entity.Model.MoveSpeed.Value;
        navMesh.SetDestination(target);
        animator.Play("Move");
    }

    public void ApplyDamage(float dmg)
    {
        if (IsDead) return;
        Entity.TakeDamaged(dmg);
    }

    public void Knockback(Vector3 dir, float power)
    {
        if (IsDead) return;
        navMesh.enabled = false;
        rigid.AddForce(dir.normalized * power, ForceMode.Impulse);
        animator.Play("Knockback");
    }

    public void Fall()
    {
        Entity.Model.Flags.SetValue(EnemyFlag.Falling, true);
        navMesh.enabled = false;
        animator.Play("Falling");
    }

    public void RecoverFromFall()
    {
        Entity.Model.Flags.SetValue(EnemyFlag.Falling, false);
        navMesh.enabled = true;
    }

    public bool IsInRange(float sqrRange)
    {
        if (player == null) return false;
        return (transform.position - player.position).sqrMagnitude < sqrRange;
    }

    private RxBehaviorNode BuildBehaviorTree()
    {
        return new Selector(new[]
        {
            Node_DeadCheck(),
            Node_FallingCheck(),
            Node_AttackCheck(),
            Node_MoveToPlayer(),
            Node_StandStill()
        });
    }

    private RxBehaviorNode Node_DeadCheck() =>
        ConditionAction.Create(() => IsDead, () =>
        {
            animator.Play("Dead");
            navMesh.isStopped = true;
        });

    private RxBehaviorNode Node_FallingCheck() =>
        ConditionAction.Create(() => IsFalling, () =>
        {
            animator.Play("Falling");
        });

    private RxBehaviorNode Node_AttackCheck() =>
        ConditionAction.Create(() => IsInRange(Entity.Model.Range.Value), () =>
        {
            navMesh.isStopped = true;
            transform.LookAt(player);
            animator.Play("Attack");
        });

    private RxBehaviorNode Node_MoveToPlayer() =>
        ConditionAction.Create(() => player != null, () =>
        {
            MoveTo(player.position);
        });

    private RxBehaviorNode Node_StandStill() =>
        ConditionAction.Create(() => true, () =>
        {
            navMesh.isStopped = true;
            animator.Play("Idle");
        });

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3.0f); // 공격 범위 디버그
    }
}