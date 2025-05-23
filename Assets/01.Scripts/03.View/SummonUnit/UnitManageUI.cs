using System.Collections;
using System.Collections.Generic;
using Ricimi;
using UnityEngine;

public class UnitManageUI : UIBase
{
    [SerializeField]
    private SummonUnitUI summonUnitUI;
    [SerializeField]
    private UnitInfo unitInfo;
    [SerializeField]
    private Transform bottom;
    [SerializeField]
    private Transform equipSlots;

    public SummonUnitUI SummonUnitUI { get { return summonUnitUI; } }

    public UnitInfo UnitInfo { get { return unitInfo; } }

    public Transform Bottom { get { return bottom; } }

    public Transform EquipSlots { get { return equipSlots; } }

    public override void HideDirect()
    {
        
    }

    public override void Opened(object[] param)
    {
        StageUIManager.Instance.unitManageUI = GetComponent<UnitManageUI>();
        StageUIManager.Instance.unitInfo = GetComponent<UnitManageUI>().UnitInfo;
        SummonUnitUI.InitUI();
        StageUIManager.Instance.RegisterUnitSlots();
        StageUIManager.Instance.RegisterEquipSlots();
        StageUIManager.Instance.InitPreviewImage();
        GameManager.Instance.PauseGame(0.2f);
        summonUnitUI.InitShop();
    }

    public override void Closed(object[] param)
    {
        SummonUnitUI.OnPopupClose();
        GameManager.Instance.PauseReleaseGame();
    }

    public void OnManageUIClose()
    {
        UIManager.Hide(this);
    }

}
