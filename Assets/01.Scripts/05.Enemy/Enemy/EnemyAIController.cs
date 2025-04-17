using System.Collections;
using UnityEngine;
using Ironcow.BT;
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
    public bool statusEffect;

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
            //Debug.Log("루프 작동");
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
    public void InDead()
    {

    }
    public void InStunned(float delayTime)
    {
        //statusEffect = true;
        rigidbodys.velocity = Vector3.zero;

    }
    public void InConfused()
    {
        statusEffect = true;
        Vector3 toTarget = transform.position - target.position;
        Vector3 ReverseTarget = transform.position + -toTarget.normalized * 5f;

        InMove(ReverseTarget);//1번 밖에 불러오지 않아 현재는 멈춰 있는 것처럼 보임, InMove 안에서 처리 하는 것으로 변경해야 할 듯
    }
    public void InKnockBack(float KnockBackDistance)
    {
 

        statusEffect = true;
        Vector3 toTarget = transform.position - target.position;
        Vector3 ReverseTarget = transform.position + toTarget.normalized * KnockBackDistance;

        transform.DOMove(ReverseTarget, 0.2f).SetEase(Ease.OutQuad);
        statusEffect = false;
    }
    public void InFalling()
    {
        if (navMesh.enabled == false)
            return;

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
            });//에어본이 중첩되는 문제있음. 추후 Ray나 다른 방법으로 막아야 함
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

    public eNodeState OnCheckDead()
    {
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
        if (statusEffect)
        {
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
        //    Debug.Log("타깃이 없다");
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

        var dir = target.position;
        InMove(dir);
        return eNodeState.running;

    }
    public eNodeState AboveStay()
    {
        Debug.Log("정지");
        navMesh.speed = 0;
        return eNodeState.running;
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
            if (GUILayout.Button("3초 스턴(개선 중)"))
            {
                ((EnemyAIController)target).InStunned(3f);
            }
            if (GUILayout.Button("혼란"))
            {
                ((EnemyAIController)target).InConfused();
            }
            if (GUILayout.Button("넉백(거리 1)"))
            {
                ((EnemyAIController)target).InKnockBack(1f);
            }
            if (GUILayout.Button("에어본"))
            {
                ((EnemyAIController)target).InFalling();
            }
        }
    }
}
