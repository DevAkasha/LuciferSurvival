using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eUnitGrade
{
    One = 1,
    Two,
    Three,
    Four,
    Five,
}

[CreateAssetMenu(fileName = "UnitDataSO", menuName = "ScriptableObjects/UnitDataSO")]
public class UnitDataSO : BaseDataSO
{
    public float range;
    public float atk;
    public float atkSpeed;
    public float criticalDamage;
    public float criticalChance;
    public int cost;
    public eUnitGrade grade;
}