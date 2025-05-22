using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulAltar : BaseInteractable
{
    [SerializeField] private UnitManageUI manageUI;

    [Header("나침반")]
    [SerializeField] private CompassArrow compassArrow;
    
    public override void Interact(PlayerEntity player)
    {
        OpenSummonUI();
    }

    private void Start()
    {
        if (compassArrow != null)
        {
            compassArrow.SetTarget(this.transform);
        }
    }

    public void OpenSummonUI()
    {
        UIManager.Show(manageUI, transform.parent);
    }
}
