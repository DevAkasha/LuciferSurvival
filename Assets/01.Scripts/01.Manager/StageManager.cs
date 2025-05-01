using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public List<UnitModel> unitList = new List<UnitModel>();

    private RxVar<int> soulStone = new RxVar<int>();

    public int SoulStone { 
        get { return soulStone.Value; }
        set { soulStone.SetValue(value, this); }
    }

    public bool UseSoulStone(int cost)
    {
        if (soulStone.Value >= cost)
        {
            SoulStone -= cost;
            return true;
        }
        return false;
    }

    public void GetSoulStone(int count)
    {
        SoulStone += count;
    }
}
