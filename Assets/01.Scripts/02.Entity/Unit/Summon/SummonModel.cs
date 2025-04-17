using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonModel : BaseModel
{
    public string rcode;
    public int level;
    public int cost;
    public int[] summonRate;

    public SummonModel(SummonTableSO summonTableSO)
    {
        rcode = summonTableSO.rcode;
        level = summonTableSO.level;
        cost = summonTableSO.cost;
        summonRate = summonTableSO.summonRate;
    }
}
