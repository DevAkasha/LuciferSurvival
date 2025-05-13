using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardDataSO", menuName = "ScriptableObjects/RewardDataSO")]
public class FarmingDataSO : BaseDataSO
{
    public string idx;          // idx
    public float probability;   // 확률
    public int gain;            // 갯수
}
