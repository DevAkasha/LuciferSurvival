using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum eUnitState
{
    Stay,               //(기본) 가만히 있을때
    Attack              //공격할때
}

public enum eProjectileType
{
    None,
    Chain,
    Spire
}

public class UnitModel : BaseModel
{
    public string rcode;
    public Sprite thumbnail;
    public string displayName;
    public string description;
    public int idx;
    public float range;
    public float atk;
    public float atkCoolTime;
    public float criticalDamage;
    public float criticalChance;
    public int cost;
    public eUnitGrade grade;
    public eUnitState unitState = eUnitState.Stay;
    public bool usePriorityTargeting = true;

    public UnitModel(UnitDataSO unitDataSO)
    {
        rcode = unitDataSO.rcode;
        thumbnail = unitDataSO.thumbnail;
        displayName = unitDataSO.displayName;
        description = unitDataSO.description;
        idx = unitDataSO.idx;
        range = unitDataSO.range;
        atk = unitDataSO.atk;
        atkCoolTime = unitDataSO.atkCoolTime;
        criticalDamage = unitDataSO.criticalDamage;
        criticalChance = unitDataSO.criticalChance;
        cost = unitDataSO.cost;
        grade = unitDataSO.grade;
        // usePriorityTargeting = unitDataSO.usePriorityTargeting; 우선도 적용여부 bool값 초기화, 추가해야함
    }

    public override IEnumerable<IModifiable> GetModifiables()
    {
        throw new NotImplementedException();
    }

}
