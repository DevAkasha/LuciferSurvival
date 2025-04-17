using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class UITest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (UnitDataSO data in SummonTableUtil.RerollShop(1))
        {
            Debug.Log(data);
        }
    }

}
