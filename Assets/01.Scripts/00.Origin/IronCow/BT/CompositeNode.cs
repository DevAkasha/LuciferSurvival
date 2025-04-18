using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CompositeNode : BTNode
{
    [SerializeReference] public List<BTNode> childs = new List<BTNode>();
}