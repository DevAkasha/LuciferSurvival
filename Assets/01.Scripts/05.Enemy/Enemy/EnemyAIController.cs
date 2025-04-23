using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;

public class EnemyAIController : MobileController<EnemyEntity, EnemyModel>
{
    //public EnemyDataSO enemyStatus;
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Rigidbody rigidbodys;
    [SerializeField] private Animator animator;
    Transform target;

    //Collider[] colliders = new Collider[10];
    //Collider TargetPlayer;

    [Header("Enemy 정보")]
    public float AttackRate;
    public bool StatusEffect;
    public bool Confused;

    [Header("BT러너")]
    [SerializeField] BTRunner bt;

    private void Start()
    {
        target = PlayerManager.Instance.Player.transform;
        rigidbodys = GetComponent<Rigidbody>();
        navMesh = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        Init();
    }
    public void Init()
    {
        bt = new BTRunner(name.Replace("(Clone)", "")).SetActions(this);
        StartCoroutine(BTLoop());
    }
    IEnumerator BTLoop()
    {
        while (true)
        {
            bt.Operate();
            Debug.Log("루프 작동");
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void InMove(Vector3 dir)
    {
        if (navMesh.enabled == false)
            return;

        rigidbodys.velocity = Vector3.zero;
        navMesh.speed = Entity.Model.MoveSpeed.Value;
        navMesh.SetDestination(dir);
        //Debug.Log("이동 중");
    }
    public void InDamage(float damage)
    {
        Entity.TakeDamaged(damage);
    }
    public void InToDamage(float damage)
    {
        PlayerManager.Instance.Player.Entity.TakeDamaged(damage);
    }
    public void InDead()
    {

    }
    public void InStunned(float delayTime)
    {
        StatusEffect = true;
        rigidbodys.velocity = Vector3.zero;
        DOVirtual.DelayedCall(delayTime, () => { InOffStatusEffect(); });

    }
    public void InConfused(float delayTime)
    {
        Confused = true;
        DOVirtual.DelayedCall(delayTime, () => { InOffStatusEffect(); });
    }
    public void InKnockBack(float KnockBackDistance)
    {
        if (StatusEffect)
            return;

        StatusEffect = true;
        Vector3 toTarget = transform.position - target.position;
        Vector3 ReverseTarget = transform.position + toTarget.normalized * KnockBackDistance;

        transform.DOMove(ReverseTarget, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            StatusEffect = false;
        }); ;
    }
    public void InFalling()
    {
        if (navMesh.enabled == false)
            return;//에어본 중첩 방지

        navMesh.enabled = false;

        transform.DOMoveY(transform.position.y + 3f, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOMoveY(1f, 0.4f).SetEase(Ease.InQuad)
                .OnComplete(() =>
                    {
                        navMesh.enabled = true;
                    });
            });
    }
    public void InDotDamage(float totalDamage, int timer)
    {
        float dotDamage = totalDamage / timer;

        DOVirtual.DelayedCall(1, () =>
            {
                Entity.TakeDamaged(dotDamage);
                Debug.Log($"{dotDamage}의 피해 ");
            })
            .SetLoops(timer, LoopType.Restart);
    }
    public void InOffStatusEffect()
    {
        StatusEffect = false;
        Confused = false;
    }

    public eNodeState OnCheckDead()
    {
        if (Entity == null || Entity.Model == null )//|| Entity.Model.Health == null)
        {
            Debug.LogError("Entity 또는 하위 필드가 null입니다.");
            return eNodeState.failure;
        }

        if (Entity.Model.Health.Value <= 0)
        {
            return eNodeState.success;
        }
        return eNodeState.failure;
    }
    public eNodeState OnDead()
    {
        navMesh.speed = 0;
        //InDead();
        return eNodeState.success;
    }
    public eNodeState IsStatusEffect()
    {
        if (StatusEffect)
        {
            navMesh.speed = 0;
            return eNodeState.success;
        }

        return eNodeState.failure;
    }
    public eNodeState DoCheakAttacking()
    {
        return eNodeState.success;
        //return eNodeState.running;
    }
    public eNodeState DoCheckPlayerWithAttackRange()
    {
        if (target == null)
        {
            return eNodeState.failure;
        }

        var dist = (transform.position - target.transform.position).sqrMagnitude;

        if (dist < Entity.Model.Range.Value)
        {
            navMesh.speed = 0;
            return eNodeState.success;
        }
        else
        {
            return eNodeState.failure;
        }
    }
    public eNodeState DoEnemyAttack()
    {
        navMesh.speed = 0;

        if (target != null)
        {
            transform.LookAt(target.transform);
            return eNodeState.success;
        }
        return eNodeState.failure;
    }
    //public eNodeState ToDetectPlayer()
    //{
    //    int count = Physics.OverlapSphereNonAlloc(transform.position, enemyStatus.detectRange,
    //        colliders, 1 << LayerMask.NameToLayer("Player"));
    //    //Debug.Log($"{count}");

    //    if (count > 0)
    //    {
    //        var list = colliders.ToList();
    //        list.RemoveAll(obj => obj == null);
    //        TargetPlayer = list.OrderBy(col => (transform.position - col.transform.position).sqrMagnitude).First();
    //        return eNodeState.success;
    //    }
    //    else
    //    {
    //        TargetPlayer = null;
    //        Debug.Log("타깃 널");
    //    }
    //    //Debug.Log("추적 실패");
    //    return eNodeState.failure;
    //}

    public eNodeState ToTargetMove()
    {
        //if (TargetPlayer == null)
        //{
            //Debug.Log("타깃이 없다");
        //    return eNodeState.failure;
        //}
        if (target == null)
        {
            Debug.Log("타깃이 없다");
            return eNodeState.failure;
        }
        if ((target.transform.position - transform.position).sqrMagnitude < Entity.Model.Range.Value)
        {
            navMesh.speed = 0;
            return eNodeState.success;
        }
        //var dir = (target.transform.position - transform.position).normalized;
            //Debug.Log("추적 개시");

        InMove(IsTargetPosition());
        return eNodeState.running;

    }
    public eNodeState AboveStay()
    {
        Debug.Log("정지");
        navMesh.speed = 0;
        return eNodeState.running;
    }
    bool IsAnimationRunning(string stateName)//애니메이션 작동 판별
    {
        if (animator != null)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                var normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                return normalizedTime != 0 && normalizedTime < 1f;
            }
        }
        return false;
    }
    Vector3 IsTargetPosition()
    {
        if (Confused)
        {
            Vector3 toTarget = transform.position - target.position;
            Vector3 ReverseTarget = transform.position + toTarget.normalized * 10f;

            return ReverseTarget;
        }
        else
        {
            var dir = target.position;

            return dir;
        }
    }
}

[CustomEditor(typeof(EnemyAIController))]
public class EnemyControl : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("한 방에 주님 곁에"))
            {
                ((EnemyAIController)target).InDamage(10000);
            }
            if (GUILayout.Button("스턴(3초)"))
            {
                ((EnemyAIController)target).InStunned(3f);
            }
            if (GUILayout.Button("혼란(3초)"))
            {
                ((EnemyAIController)target).InConfused(3);
            }
            if (GUILayout.Button("넉백(거리 1.5)"))
            {
                ((EnemyAIController)target).InKnockBack(1.5f);
            }
            if (GUILayout.Button("에어본"))
            {
                ((EnemyAIController)target).InFalling();
            }
            if (GUILayout.Button("지속 피해(300, 3초)"))
            {
                ((EnemyAIController)target).InDotDamage(300, 3);
            }
        }
    }
}
