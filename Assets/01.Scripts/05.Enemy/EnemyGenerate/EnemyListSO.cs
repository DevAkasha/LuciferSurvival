using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyListSO", menuName = "ScriptableObjects/EnemyListSO")]
public class EnemyListSO : ScriptableObject
{
    public List<GameObject> SpawnEnemyList;
}
