using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Ironcow.BT;

public class EnemyAIController : MonoBehaviour
{
    public EnemyStatus enemyInfo;
    [SerializeField] private Rigidbody rigidbodys;

    [SerializeField] BTRunner bt;
    Collider attackTarget;

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
            yield return new WaitForSeconds(0.1f);
        }
    }

    public eNodeState DoEnemyAttack()
    {
        rigidbodys.velocity = Vector3.zero;
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
            return eNodeState.success;
        }
        return eNodeState.failure;
    }
}
