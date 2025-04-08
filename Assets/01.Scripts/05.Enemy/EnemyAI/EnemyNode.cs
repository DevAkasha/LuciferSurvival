using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    ChaseState,
    AttackState
}

[SerializeField]
public abstract class EnemyNode
{
    public string className;
    public EnemyNode parent;
    public abstract NodeState Evaluate();
    public abstract EnemyNode AddAction(Func<NodeState> func);
    public abstract EnemyNode AddSequence();
    public abstract EnemyNode AddSelector();
    public abstract Type GetRealType();
}
