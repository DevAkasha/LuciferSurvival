using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionTableList : MonoBehaviour
{
    [SerializeField]
    private GameObject unionCardPrefab;

    [SerializeField]
    private Transform listTransform;

    private List<UnionTableSO> unionTables;

    private void OnEnable()
    {
        unionTables = DataManager.Instance.GetDatas<UnionTableSO>();
        if (listTransform.GetComponentsInChildren<UnionCard>().Length > 0)
        {
            //이미 값이 있는경우
        }
        else
        {
            
        }
    }


}
