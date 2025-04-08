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

[CreateAssetMenu(fileName = "EnemyInfo", menuName = "ScriptableObjects/EnemyInfo")]
public class EnemyInfo : ScriptableObject
{
    public string EnemyName = "전갱이";
    public EnemyType enemyType;
    public RaceType raceType;
    public JobType jobType;
    public float moveSpeed = 1f;
}
