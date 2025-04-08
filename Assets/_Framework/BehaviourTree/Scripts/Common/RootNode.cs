using System;
using UnityEngine;

namespace Ironcow.BT
{
    [Serializable]
    public class RootNode : BTNode
    {
        [SerializeReference] public BTNode node;

        public RootNode()
        {
            className = GetType().Name;
        }

        public override BTNode AddAction(Func<eNodeState> func)
        {
            node.AddAction(func);
            return this;
        }

        public override BTNode AddSelector()
        {
            node.AddSelector();
            return this;
        }

        public override BTNode AddSequence()
        {
            node.AddSequence();
            return this;
        }

        public override eNodeState Evaluate()
        {
            return node.Evaluate();
        }

        public override Type GetRealType()
        {
            return GetType();
        }
    }
}