using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionTableList : MonoBehaviour
{
    [SerializeField]
    private UnionCard unionCardPrefab;

    [SerializeField]
    private Transform listTransform;

    private List<UnionTableSO> unionTables;

    private void OnEnable()
    {
        unionTables = DataManager.Instance.GetDatas<UnionTableSO>();
        if (listTransform.GetComponentsInChildren<UnionCard>().Length == 0)
        {
            for (int i = 0; i < unionTables.Count; i++)
            {
                UnionCard card = Instantiate(unionCardPrefab, listTransform);
                card.SetUnionModel(unionTables[i].rcode);
            }
        }
    }


}
