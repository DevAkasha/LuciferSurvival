using System.Collections;
using System.Collections.Generic;
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

    public void SetUnionDetail(UnionTableSO tableSO)
    {
        UnitDataSO unitData = DataManager.Instance.GetData<UnitDataSO>(tableSO.unitRcode);
        detailCard.SetUnitImage(unitData.thumbnail);
        detailCard.SetUnitName(unitData.displayName);
        ClearChildren();

        string[] unitList = { tableSO.unit1, tableSO.unit2, tableSO.unit3, tableSO.unit4 };

        if (!tableSO.unit4.Equals(string.Empty))
        {
            for (int i = 0; i < 3; i++)
            {
                MaterialCard card = Instantiate<MaterialCard>(materialPrefab, materialTransform);
                UnitDataSO materialData = DataManager.Instance.GetData<UnitDataSO>(unitList[i]);
                card.SetUnitImage(materialData.thumbnail);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                MaterialCard card = Instantiate<MaterialCard>(materialPrefab, materialTransform);
                UnitDataSO materialData = DataManager.Instance.GetData<UnitDataSO>(unitList[i]);
                card.SetUnitImage(materialData.thumbnail);
            }
        }
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
}
