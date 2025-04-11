using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Normal,
    Elite,
    Boss
}
public enum RaceType
{
    Angle
}
public enum JobType
{
    None
}

[CreateAssetMenu(fileName = "EnemyStatus", menuName = "ScriptableObjects/EnemyStatus")]
public class EnemyStatus : ScriptableObject
{
    public string EnemyName = "전갱이";
    public EnemyType enemyType;
    public RaceType raceType;
    public JobType jobType;
    public int MaxHp = 100;
    public float moveSpeed = 1f;
    public float attackRange = 5f;
    public float detectRange = 10f;
}
