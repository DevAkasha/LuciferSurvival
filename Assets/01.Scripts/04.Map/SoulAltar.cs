using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulAltar : BaseInteractable
{
    [SerializeField] private UnitManageUI manageUI;
    [SerializeField] private Transform canvas;
    
    public override void Interact(PlayerEntity player)
    {
        OpenSummonUI();
    }

    public void OpenSummonUI()
    { 
        canvas = StageUIManager.Instance.GetCanvasTransform();
        UIManager.Show(manageUI, canvas);
    }
}
