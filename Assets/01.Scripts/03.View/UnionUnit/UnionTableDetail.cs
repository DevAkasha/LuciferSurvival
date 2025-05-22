using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UnionTableDetail : MonoBehaviour
{
    [SerializeField]
    private MaterialCard materialPrefab;

    [SerializeField]
    private UnionCard detailCard;

    [SerializeField]
    private Transform materialTransform;

    [SerializeField]
    private TextMeshProUGUI unionBtnText;

    private UnionTableSO curTableData;

    public UnionTableSO CurTable { get { return curTableData; } }

    private UnitDataSO curUnitData;

    public UnitDataSO CurUnit { get { return curUnitData; } }

    public void SetUnionDetail(UnionTableSO tableSO)
    {
        UnitDataSO unitData = DataManager.Instance.GetData<UnitDataSO>(tableSO.unitRcode);
        curTableData = tableSO;
        curUnitData = unitData;
        SetBtnText(curUnitData.cost);
        detailCard.SetUnitImage(curUnitData.thumbnail);
        detailCard.SetUnitName(curUnitData.displayName);
        RefreshMaterials();
    }

    //구성요소 초기화
    public void ClearChildren()
    {
        for (int i = 0; i < materialTransform.childCount; i++)
        {
            GameObject obj = materialTransform.GetChild(i).gameObject;
            Destroy(obj);
        }
    }

    public void RefreshMaterials()
    {
        string[] unitList = { curTableData.unit1, curTableData.unit2, curTableData.unit3, curTableData.unit4 };

        int spawnCount = string.IsNullOrEmpty(curTableData.unit4) ? 3 : 4;

        ClearChildren();
        for (int i = 0; i < spawnCount; i++)
        {
            MaterialCard card = Instantiate(materialPrefab, materialTransform);
            UnitDataSO materialData = DataManager.Instance.GetData<UnitDataSO>(unitList[i]);
            card.SetUnitImage(materialData.thumbnail);
            card.SetIsOwned(UnitManager.Instance.CheckUnit(unitList[i]));
        }
    }

    public void SetBtnText(int cost)
    {
        unionBtnText.text = "합성(비용 : " + cost + ")";
    }
}
