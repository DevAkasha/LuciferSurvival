using UnityEngine;

public abstract class UnitSlotBase : MonoBehaviour
{
    protected StackableUnitModel stackableUnit;
    protected int slotIndex;

    public virtual void SetSlot(StackableUnitModel unitModel, int index)
    {
        stackableUnit = unitModel;
        slotIndex = index;
        UpdateSlotUI();
    }

    public virtual void ClearSlot()
    {
        stackableUnit = null;
        UpdateSlotUI();
    }

    protected abstract void UpdateSlotUI();
}