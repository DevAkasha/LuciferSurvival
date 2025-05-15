using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionTableMain : MonoBehaviour
{
    [SerializeField]
    private UnionTableDetail unionTableDetail;

    [SerializeField]
    private UnionTableList unionTableList;

    [SerializeField]
    private UnitInfo unitInfo;

    public void ShowTableDetail()
    {
        unionTableDetail.gameObject.SetActive(true);
    }

    public void SetTableDetail(UnionTableSO tableSO)
    {
        unionTableDetail.SetUnionDetail(tableSO);
    }

    public void SetUnitInfo(UnitDataSO unitData)
    {
        unitInfo.SetUnitInfo(new UnitModel(unitData));
        unitInfo.SetUnitSkillInfo();
    }
}
