using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Ironcow.BT;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;

public class EnemyAIController : MonoBehaviour
{
    public EnemyStatus enemyStatus;
    public int Hp;
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Animator animator;

    [SerializeField] BTRunner bt;
    //Collider[] colliders = new Collider[10];
    //Collider TargetPlayer;
    Transform target;

    private void Start()
    {
        target = PlayerManager.Instance.Player.transform;
        navMesh = GetComponent<NavMeshAgent>();
        Hp = enemyStatus.MaxHp;

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
        //Debug.Log("이동 중");
        navMesh.speed = enemyStatus.moveSpeed;
        navMesh.SetDestination(dir);
    }
    public void InDamage(int damage)
    {
        Hp -= damage;

        if (Hp <= 0)
        {
            InDead();
        }
    }
    public void InDead()
    {

    }
    public eNodeState OnCheckDead()
    {
        if (Hp <= 0)
        {
            return eNodeState.success;
        }
        return eNodeState.failure;
    }
    public eNodeState OnDead()
    {
        navMesh.speed = 0;
        return eNodeState.success;
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

        if (dist < enemyStatus.attackRange)
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
        if ((target.transform.position - transform.position).sqrMagnitude < enemyStatus.attackRange)
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyStatus.attackRange);
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
        }
    }
}
