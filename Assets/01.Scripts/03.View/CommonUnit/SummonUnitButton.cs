using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonUnitButton : MonoBehaviour
{
    [SerializeField]
    private UnitManageUI manageUI;

    public void OpenSummonUI()
    {
        UIManager.Show(manageUI, transform.parent);
    }
}
