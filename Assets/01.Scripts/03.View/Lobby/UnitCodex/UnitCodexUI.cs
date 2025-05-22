using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitCodexUI : UIBase
{
    [SerializeField]
    private UnitCard cardPrefab;

    [SerializeField]
    private Transform contentTransform;

    public override void HideDirect()
    {
        
    }

    public override void Opened(object[] param)
    {
        List<UnitDataSO> units = DataManager.Instance.GetDatas<UnitDataSO>()
        .OrderByDescending(m => m.grade)   // 높은 등급 우선
        .ToList();

        foreach (UnitDataSO data in units)
        {
            UnitCard card = Instantiate(cardPrefab, contentTransform);
            card.SetUnitCard(data);
            card.SetDetailTransform(transform.parent);
        }
    }
}
