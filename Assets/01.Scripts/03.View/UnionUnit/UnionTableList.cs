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
        RefreshList();
    }

    public void RefreshList()
    {
        foreach (Transform child in listTransform)
            Destroy(child.gameObject);

        unionTables = DataManager.Instance.GetDatas<UnionTableSO>();

        for (int i = 0; i < unionTables.Count; i++)
        {
            var table = unionTables[i];
            UnionCard card = Instantiate(unionCardPrefab, listTransform);
            card.SetUnionData(table.rcode);
            card.SetCompletionRate(UnitManager.Instance.CalculateCompletionRate(table));
        }
    }
}
