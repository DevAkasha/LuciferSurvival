using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public RxVar<int> soulStone = new RxVar<int>(0); //@SM           //게임 내 재화(초기값 : 0)
 
    protected override void Awake()
    {
        base.Awake();
        SoulStone = 9; //테스트 코드인가요?
    }

    public int SoulStone 
    {
        get { return soulStone.Value; }
        set { soulStone.SetValue(value, this); }
    }

    public bool ReduceSoulStone(int amount)
    {
        if (soulStone.Value >= amount)
        {
            SoulStone -= amount;
            return true;
        }
        return false;
    }

}
