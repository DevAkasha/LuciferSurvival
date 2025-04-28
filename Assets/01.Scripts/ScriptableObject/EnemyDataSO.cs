using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ScriptableObjects/EnemyDataSO")]
public class EnemyDataSO : BaseDataSO
{
    public int idx;
    public float atk;
    public float moveSpeed;
    public float health;
    public float atkRange;

}
