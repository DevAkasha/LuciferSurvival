using System.Collections;
using System.Collections.Generic;
using Ironcow.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "ScriptableObjects/EnemyDataSO")]
public class EnemyDataSO : BaseDataSO
{
    public int idx;
    public float atk;
    public float moveSpeed;
    public float health;
}
