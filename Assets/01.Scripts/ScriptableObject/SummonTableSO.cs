using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonTableSO", menuName = "ScriptableObjects/SummonTableSO")]
public class SummonTableSO : BaseDataSO
{
    public int level;
    public int cost;
    public int[] summonRate;
}
