using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AtkType
{
    standard,
    tank,
    dasher,
    rusher,
    shooter
}
[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ScriptableObjects/EnemyDataSO")]
public class EnemyDataSO : BaseDataSO
{
    public int idx;
    public AtkType atkType;
    public float atk;
    public float moveSpeed;
    public float health;
    public float atkRange;
    public float coolTime;
}
