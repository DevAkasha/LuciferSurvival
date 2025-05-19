using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCodexUI : UIBase
{
    [SerializeField]
    private UnitCard cardPrefab;

    public override void HideDirect()
    {
        
    }

    public override void Opened(object[] param)
    {
        List<UnitDataSO> units = DataManager.Instance.GetDatas<UnitDataSO>();

        foreach (UnitDataSO data in units)
        {

        }
    }
}
