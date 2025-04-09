using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using Ironcow.Data;

[CreateAssetMenu(fileName = "UnitDataSO", menuName = "ScriptableObjects/UnitDataSO")]
public class UnitDataSO : BaseDataSO
{
    public int idx;
    public float range;
    public float atk;
    public float atkSpeed;
    public float criticalDamage;
    public float criticalChance;
}