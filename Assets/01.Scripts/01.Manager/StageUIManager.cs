using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : Singleton<StageUIManager>
{
    private UnitSlot[] unitSlotUIs = new UnitSlot[8];

    private EquipSlot[] equipSlotUIs = new EquipSlot[6];  // 동적 할당 가능하게 변경

    [SerializeField]
    private Canvas canvas; // 드래그 프리뷰 위치 계산용 (캔버스 추가 필요)

    [SerializeField]
    private Image activeDragPreview; // 현재 드래그 중인 프리뷰 아이콘

    public UnitInfo unitInfo;

    [SerializeField]
    private GameObject slotPrefab;

    public UnitManageUI unitManageUI;

    public UnitInfo UnitInfo { get { return unitInfo; } }

    protected override void Awake()
    {
        base.Awake();
        activeDragPreview.gameObject.SetActive(false);
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
            equipSlotUIs[i].SetSlot(StageManager.Instance.GetEquippedUnit(i));
        }
        RefreshAllEquipSlots();
    }

    public void RefreshAllUnitSlots()
    {
        UnitInventory[] unitSlots = StageManager.Instance.unitSlots;

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
        UnitInventory[] unitSlots = StageManager.Instance.unitSlots;

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
        EquipSlot[] equipSlots = StageUIManager.Instance.equipSlotUIs;

        if (index < 0 || index >= equipSlots.Length)
            return;

        UnitModel unit = StageManager.Instance.GetEquippedUnit(index);
        equipSlots[index].SetSlot(unit);
    }

    public void RefreshAllEquipSlots()
    {
        for (int i = 0; i < equipSlotUIs.Length; i++)
        {
            RefreshEquipSlot(i);
        }
    }
}
