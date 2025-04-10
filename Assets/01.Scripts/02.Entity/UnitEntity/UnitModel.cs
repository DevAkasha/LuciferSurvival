using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModel : BaseModel
{
    public string rcode;
    public string displayName;
    public int idx;
    public float range;
    public float atk;
    public float atkSpeed;
    public float criticalDamage;
    public float criticalChance;
    public int cost;
    public UnitGrade grade;

    public UnitModel(UnitDataSO unitDataSO)
    {
        rcode = unitDataSO.rcode;
        displayName = unitDataSO.displayName;
        idx = unitDataSO.idx;
        range = unitDataSO.range;
        atk = unitDataSO.atk;
        atkSpeed = unitDataSO.atkSpeed;
        criticalDamage = unitDataSO.criticalDamage;
        criticalChance = unitDataSO.criticalChance;
        cost = unitDataSO.cost;
        grade = unitDataSO.grade;
    }

    public override IEnumerable<IModifiable> GetModifiables()
    {
        throw new NotImplementedException();
    }

}
