using System.Collections;
using System.Collections.Generic;
using Ironcow.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonTableSO", menuName = "ScriptableObjects/SummonTableSO")]
public class SummonTableSO : BaseDataSO
{
    public string rcode;
    public int level;
    public int cost;
    public float[] summonRate;
}
