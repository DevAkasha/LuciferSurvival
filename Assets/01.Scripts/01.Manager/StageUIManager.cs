using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : Singleton<StageUIManager>
{
    private UnitSlot[] unitSlotUIs = new UnitSlot[8];

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

    public void Register()
    {
        for (int i = 0; i < unitSlotUIs.Length; i++)
        {
            GameObject go = Instantiate(slotPrefab, unitManageUI.Bottom);
            UnitSlot slot = go.GetComponent<UnitSlot>();

            unitSlotUIs[i] = slot;
        }
        RefreshAllUnitSlots();
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
        var unitSlots = StageManager.Instance.unitSlots;

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
}
