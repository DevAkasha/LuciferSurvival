using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRootsNode : EnemyNode
{
    [SerializeReference] public EnemyNode node;
    public EnemyRootsNode()
    {
        className = GetType().Name;
    }
    public override EnemyNode AddAction(Func<NodeState> func)
    {
        return this;
    }
    public override EnemyNode AddSequence()
    {
        return this;
    }
    public override EnemyNode AddSelector()
    {
        return this;
    }
    public override NodeState Evaluate()
    {
        return node.Evaluate();
    }
    public override Type GetRealType()
    {
        return GetType();
    }
}
