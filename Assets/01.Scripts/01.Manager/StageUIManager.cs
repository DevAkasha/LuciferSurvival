using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : Singleton<StageUIManager>
{
    protected override bool IsPersistent => false;
    
    private UnitSlot[] unitSlotUIs = new UnitSlot[8];

    private EquipSlot[] equipSlotUIs = new EquipSlot[6];  // 동적 할당 가능하게 변경

    [SerializeField] private Canvas canvas; // 드래그 프리뷰 위치 계산용 (캔버스 추가 필요)

    private Image activeDragPreview; // 현재 드래그 중인 프리뷰 아이콘

    public UnitInfo unitInfo;

    [SerializeField] private GameObject slotPrefab;

    public UnitManageUI unitManageUI;

    private HashSet<string> unionFavoriteSet = new HashSet<string>();

    public UnitInfo UnitInfo { get { return unitInfo; } }

    public HashSet<string> UnionFavoriteSet { get { return unionFavoriteSet; } }

    [SerializeField] private GameObject StageCleatWindow;
    [SerializeField] private GameObject PlayerDeathWindow;

    [SerializeField] private GameObject BossWarningView;

    protected override void Awake()
    {
        base.Awake();
    }

    public void RegisterUnitSlots()
    {
        for (int i = 0; i < unitSlotUIs.Length; i++)
        {
            
            GameObject go = Instantiate(slotPrefab, unitManageUI.Bottom);
            UnitSlot slot = go.GetComponent<UnitSlot>();

            unitSlotUIs[i] = slot;
        }
        RefreshAllUnitSlots();
    }

    public void RegisterEquipSlots()
    {
        equipSlotUIs = unitManageUI.EquipSlots.GetComponentsInChildren<EquipSlot>(true);

        for (int i = 0; i < equipSlotUIs.Length; i++)
        {
            equipSlotUIs[i].SetSlot(UnitManager.Instance.GetEquippedUnit(i));
        }
        RefreshAllEquipSlots();
    }

    public void InitPreviewImage()
    {
        if (activeDragPreview != null)
        {
            activeDragPreview.transform.SetAsLastSibling();
            return;
        }
            
        GameObject imagePrefab = new GameObject("BasicImage", typeof(Image));

        activeDragPreview = Instantiate(imagePrefab, canvas.transform).GetComponent<Image>();
        activeDragPreview.rectTransform.sizeDelta = new Vector2(100, 100);
        activeDragPreview.raycastTarget = false;
        activeDragPreview.gameObject.SetActive(false);
    }

    public void RefreshAllUnitSlots()
    {
        StackableUnitModel[] unitSlots = UnitManager.Instance.curUnitArray;

        for (int i = 0; i < unitSlotUIs.Length; i++)
        {
            if (unitSlotUIs[i] != null)
            {
                unitSlotUIs[i].SetSlot(unitSlots[i], i);
            }
        }
    }

    public void RefreshUnitSlot(int index)
    {
        StackableUnitModel[] unitSlots = UnitManager.Instance.curUnitArray;

        if (index < 0 || index >= unitSlotUIs.Length)
            return;

        if (unitSlotUIs[index] != null)
        {
            unitSlotUIs[index].SetSlot(unitSlots[index], index);
        }
    }

    public void ShowDragPreview(Sprite icon)
    {
        if (activeDragPreview == null || icon == null)
            return;

        activeDragPreview.sprite = icon;
        activeDragPreview.gameObject.SetActive(true);
    }

    public void UpdateDragPreviewPosition()
    {
        if (activeDragPreview == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 localPos
        );

        activeDragPreview.rectTransform.anchoredPosition = localPos;
    }

    public void HideDragPreview()
    {
        if (activeDragPreview != null)
        {
            activeDragPreview.gameObject.SetActive(false);
        }
    }

    public void RefreshEquipSlot(int index)
    {
        EquipSlot[] equipSlots = equipSlotUIs;

        if (equipSlotUIs[index] == null)
            return;

        UnitModel unit = UnitManager.Instance.GetEquippedUnit(index);
        equipSlots[index].SetSlot(unit);
    }

    public void RefreshAllEquipSlots()
    {
        for (int i = 0; i < equipSlotUIs.Length; i++)
        {
            RefreshEquipSlot(i);
        }
    }

    public void OnStageCleatWindow()
    {
        Instantiate(StageCleatWindow, canvas.transform);
    }

    public void OnPlayerDeathWindow()
    {
        Instantiate(PlayerDeathWindow, canvas.transform);
    }

    public void OnBossWarning()
    { 
        var StageSlear = Instantiate(BossWarningView, canvas.transform);
    }

    public Transform GetCanvasTransform()
    {
        return canvas.transform;
    }
}
