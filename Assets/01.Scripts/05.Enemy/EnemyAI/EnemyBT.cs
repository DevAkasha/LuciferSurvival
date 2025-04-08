using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBT
{
    [SerializeReference] public EnemyRootsNode rootsNode;
    public EnemyBT()
    {
        if(rootsNode ==null)
        {
            rootsNode = new EnemyRootsNode();
        }
    }
}
