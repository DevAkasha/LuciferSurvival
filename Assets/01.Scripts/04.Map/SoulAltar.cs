using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulAltar : BaseInteractable
{
    [SerializeField] private UnitManageUI manageUI;
    [SerializeField] private Transform canvas;
    [Header("나침반")]
    [SerializeField] private CompassArrow compassArrow;
    
    public override void Interact(PlayerEntity player)
    {
        OpenSummonUI();
    }

    private void Start()
    {
        compassArrow.SetTarget(this.gameObject.transform);
    }

    public void OpenSummonUI()
    {
        UIManager.Show(manageUI, canvas);
    }
}
