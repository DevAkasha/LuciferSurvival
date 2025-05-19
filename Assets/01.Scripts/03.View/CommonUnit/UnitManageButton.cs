using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManageButton : MonoBehaviour
{
    [SerializeField]
    private Transform parent;

    [SerializeField]
    private UnitManageUI ui;

    public void OnUnitManageOpen()
    {
        UIManager.Show<UIBase>(ui, parent);
    }
}
