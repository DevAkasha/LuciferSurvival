using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Ironcow.BT;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    public EnemyStatus enemyStatus;
    public int Hp;
    [SerializeField] private Rigidbody rigidbodys;
    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Animator animator;

    [SerializeField] BTRunner bt;
    Collider[] colliders = new Collider[10];
    Collider TargetPlayer;
    Transform target;

    private void Start()
    {
        target = PlayerManager.Instance.Player.transform;
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
        if (dir.sqrMagnitude > 0.01f)
        {
            Vector3 lookDir = new Vector3(dir.x, 0, dir.z); // y축 고정
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 18f);
        }
        rigidbodys.velocity = dir * enemyStatus.moveSpeed;
    }
    public void InDamage(int damage)
    {
        Hp -= damage;

        if(Hp <= 0)
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
        return eNodeState.success;
    }
    public eNodeState DoCheakAttacking()
    {
        return eNodeState.running;
    }
    public eNodeState DoCheckPlayerWithAttackRange()
    {
        if (TargetPlayer == null)
        {
            return eNodeState.failure;
        }

        var dist = (transform.position - TargetPlayer.transform.position).sqrMagnitude;

        if (dist < enemyStatus.attackRange)
        {
            rigidbodys.velocity = Vector3.zero;
            return eNodeState.success;
        }
        else
        {
            return eNodeState.failure;
        }
    }
    public eNodeState DoEnemyAttack()
    {
        rigidbodys.velocity = Vector3.zero;

        if (TargetPlayer != null)
        {
            transform.LookAt(TargetPlayer.transform);
            return eNodeState.success;
        }
        return eNodeState.failure;
    }
    public eNodeState ToDetectPlayer()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, enemyStatus.detectRange,
            colliders, 1 << LayerMask.NameToLayer("Player"));
        //Debug.Log($"{count}");

        if (count > 0)
        {
            var list = colliders.ToList();
            list.RemoveAll(obj => obj == null);
            TargetPlayer = list.OrderBy(col => (transform.position - col.transform.position).sqrMagnitude).First();
            return eNodeState.success;
        }
        else
        {
            TargetPlayer = null;
            Debug.Log("타깃 널");
        }
        //Debug.Log("추적 실패");
        return eNodeState.failure;
    }
    
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
            rigidbodys.velocity = Vector3.zero;
            return eNodeState.success;
        }
        var dir = (target.transform.position - transform.position).normalized;
        Debug.Log("추적 개시");
        InMove(dir);
        return eNodeState.running;

    }
    public eNodeState AboveStay()
    {
        Debug.Log("정지");
        rigidbodys.velocity = Vector3.zero;
        return eNodeState.running;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, enemyStatus.detectRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyStatus.attackRange);
    }
}
