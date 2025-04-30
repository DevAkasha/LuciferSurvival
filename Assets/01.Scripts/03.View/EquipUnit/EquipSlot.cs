using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : UnitSlotBase, IPointerClickHandler, IPointerEnterHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public static EquipSlot draggingEquipSlot = null;

    [SerializeField]
    private Image iconImage;

    private UnitModel equippedUnit;

    public void SetSlot(UnitModel unit)
    {
        equippedUnit = unit;

        if (unit != null)
        {
            iconImage.sprite = unit.thumbnail;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    protected override void UpdateSlotUI()
    {
        if (equippedUnit != null)
        {
            iconImage.sprite = equippedUnit.thumbnail;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (StageManager.Instance.unitSlots[slotIndex] != null)
        {
            StageUIManager.Instance.UnitInfo.SetUnitInfo(StageManager.Instance.unitSlots[slotIndex].unitModel);
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //PC 기준 테두리 활성화 처리
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (equippedUnit == null) return;

        draggingEquipSlot = this;
        StageUIManager.Instance.ShowDragPreview(equippedUnit.thumbnail);
    }

    public void OnDrag(PointerEventData eventData)
    {
        StageUIManager.Instance.UpdateDragPreviewPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingEquipSlot == this)
        {
            Debug.Log("[EquipSlot] 빈 공간 드랍 → 장착 해제");
            StageManager.Instance.AddUnit(equippedUnit);
            SetSlot(null);
        }

        draggingEquipSlot = null;
        StageUIManager.Instance.HideDragPreview();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (UnitSlot.draggingSlotIndex != -1)
        {
            var draggedInventory = StageManager.Instance.unitSlots[UnitSlot.draggingSlotIndex];
            if (draggedInventory == null) return;

            UnitModel draggedUnit = draggedInventory.unitModel;

            if (equippedUnit != null)
                StageManager.Instance.AddUnit(equippedUnit);

            SetSlot(draggedUnit);
            StageManager.Instance.unitSlots[UnitSlot.draggingSlotIndex] = null;
            StageUIManager.Instance.RefreshUnitSlot(UnitSlot.draggingSlotIndex);

            UnitSlot.isDropComplete = true;
            UnitSlot.draggingSlotIndex = -1;
            return;
        }

        if (draggingEquipSlot != null && draggingEquipSlot != this)
        {
            var temp = equippedUnit;
            SetSlot(draggingEquipSlot.equippedUnit);
            draggingEquipSlot.SetSlot(temp);

            draggingEquipSlot = null;
        }

        StageUIManager.Instance.HideDragPreview();
    }
}
