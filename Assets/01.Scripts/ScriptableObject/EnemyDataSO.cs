using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    standard,
    tank,
    dasher,
    rusher,
    shooter,
    boss
}
[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ScriptableObjects/EnemyDataSO")]
public class EnemyDataSO : BaseDataSO
{
    public int idx;
    public EnemyType enemyType;
    public float atk;
    public float moveSpeed;
    public float health;
    public float atkRange;
    public float coolTime;
}
