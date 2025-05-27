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

    [SerializeField]
    private int transformIndex;

    [SerializeField]
    private Sprite defaultSprite;

    private UnitModel equippedUnit;

    public void SetSlot(UnitModel unit)
    {
        equippedUnit = unit;
        UnitManager.Instance.equippedUnitArray[transformIndex] = unit;

        if (unit != null)
        {
            iconImage.sprite = unit.thumbnail;
        }
        else
        {
            iconImage.sprite = defaultSprite;
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
        if (equippedUnit != null)
        {
            StageUIManager.Instance.UnitInfo.SetUnitInfo(equippedUnit);
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
            UnitManager.Instance.UnequipUnit(transformIndex);
        }

        draggingEquipSlot = null;
        StageUIManager.Instance.HideDragPreview();
        StageUIManager.Instance.RefreshAllUnitSlots();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (UnitSlot.draggingSlotIndex != -1)
        {
            var draggedInventory = UnitManager.Instance.curUnitArray[UnitSlot.draggingSlotIndex];
            if (draggedInventory == null) return;

            UnitModel draggedUnit = draggedInventory.unitModel;

            if (equippedUnit != null)
                UnitManager.Instance.AddUnit(equippedUnit);

            // draggedInventory.count 줄이기
            if (draggedInventory.count > 1)
            {
                draggedInventory.count--;
            }
            else
            {
                UnitManager.Instance.curUnitArray[UnitSlot.draggingSlotIndex] = null;
            }

            SetSlot(draggedUnit); // 장착
            StageUIManager.Instance.RefreshAllUnitSlots();

            PlayerManager.Instance.Player.AddUnitTransform(transformIndex, equippedUnit);

            UnitSlot.isDropComplete = true;
            UnitSlot.draggingSlotIndex = -1;
            StageUIManager.Instance.HideDragPreview();
            return;
        }

        if (draggingEquipSlot != null && draggingEquipSlot != this)
        {
            int fromIndex = draggingEquipSlot.transformIndex;   //드래그중인 인덱스
            int toIndex = transformIndex;                   //현재 인덱스

            var temp = equippedUnit;
            SetSlot(draggingEquipSlot.equippedUnit);
            draggingEquipSlot.SetSlot(temp);

            PlayerManager.Instance.Player.AddUnitTransform(fromIndex, equippedUnit);
            PlayerManager.Instance.Player.AddUnitTransform(toIndex, draggingEquipSlot.equippedUnit);

            draggingEquipSlot = null;
            StageUIManager.Instance.HideDragPreview();
            return;
        }
    }
}
