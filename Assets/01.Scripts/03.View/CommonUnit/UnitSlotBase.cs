using UnityEngine;

public abstract class UnitSlotBase : MonoBehaviour
{
    protected UnitInventory unitInventory;
    protected int slotIndex;

    public virtual void SetSlot(UnitInventory inventory, int index)
    {
        unitInventory = inventory;
        slotIndex = index;
        UpdateSlotUI();
    }

    public virtual void ClearSlot()
    {
        unitInventory = null;
        UpdateSlotUI();
    }

    protected abstract void UpdateSlotUI();
}