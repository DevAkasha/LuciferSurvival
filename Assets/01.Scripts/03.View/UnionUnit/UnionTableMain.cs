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

    private void OnEnable()
    {
        unionTableList.RefreshList();
    }

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
        UnitModel data = new UnitModel(unitData);
        unitInfo.SetUnitInfo(data);
        unitInfo.SetUnitSkillInfo(data);
    }

    public void OnClickCombine()
    {
        bool success = StageManager.Instance.CombineUnit(unionTableDetail.CurTable);

        if (success)
        {
            StageUIManager.Instance.RefreshAllUnitSlots();
            StageUIManager.Instance.RefreshAllEquipSlots();
            unionTableList.RefreshList();
            unionTableDetail.RefreshMaterials();
        }
        else
        {
            // 조합 실패(재료 부족 또는 슬롯 공간 없음) 시 처리
            Debug.LogWarning("조합을 실행할 수 없습니다: 재료가 부족하거나 인벤토리 공간이 없습니다.");
        }
    }
}
