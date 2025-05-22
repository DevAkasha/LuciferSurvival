using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSlot : UnitSlotBase, IPointerClickHandler, IPointerEnterHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TMP_Text countText;
    [SerializeField]
    private GameObject countBackground;
    [SerializeField]
    private Image outline;

    public static int draggingSlotIndex = -1;
    public static bool isDropComplete = false;

    protected override void UpdateSlotUI()
    {
        if (stackableUnit != null)
        {
            countBackground.SetActive(true);
            iconImage.sprite = stackableUnit.unitModel.thumbnail;
            iconImage.color = Color.white;
            iconImage.enabled = true;
            countText.text = stackableUnit.count.ToString();
        }
        else
        {
            countBackground.SetActive(false);
            iconImage.sprite = null;
            iconImage.color = new Color(1f,1f,1f,0f);
            iconImage.enabled = false;
            countText.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(StageManager.Instance.curUnitArray[slotIndex] != null)
        {
            ActiveSlot();
            StageUIManager.Instance.UnitInfo.SetUnitInfo(StageManager.Instance.curUnitArray[slotIndex].unitModel);
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //PC 기준 테두리 활성화 처리
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (stackableUnit == null)
            return;

        draggingSlotIndex = slotIndex;

        StageUIManager.Instance.ShowDragPreview(stackableUnit.unitModel.thumbnail);
    }

    public void OnDrag(PointerEventData eventData)
    {
        StageUIManager.Instance.UpdateDragPreviewPosition();
    }

    public void OnEndDrag(PointerEventData eventData)               //그 이외에 드래그가 끝나면 처리해야될 것들(드래그 실패)
    {
        //테두리 비활성화 처리 필요
        if (stackableUnit == null)
            return;

        if (!isDropComplete)
        {
            Debug.Log("[UnitSlot] 드래그 실패 (빈 공간 등에 드롭됨)");
            // 필요하면 드래그 실패 애니메이션 또는 처리 추가 가능
        }

        StageUIManager.Instance.HideDragPreview();

        draggingSlotIndex = -1;
        isDropComplete = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggingSlotIndex == -1)
            return;

        if (draggingSlotIndex == slotIndex)
        {
            return;
        }

        var draggedStackableModel = StageManager.Instance.curUnitArray[draggingSlotIndex];
        var targetStackableModel = StageManager.Instance.curUnitArray[slotIndex];

        if (draggedStackableModel == null)
        {
            return;
        }

        if (targetStackableModel == null)
        {
            StageManager.Instance.curUnitArray[slotIndex] = draggedStackableModel;
            StageManager.Instance.curUnitArray[draggingSlotIndex] = null;
        }
        else
        {
            StageManager.Instance.curUnitArray[draggingSlotIndex] = targetStackableModel;
            StageManager.Instance.curUnitArray[slotIndex] = draggedStackableModel;
        }

        StageUIManager.Instance.RefreshUnitSlot(draggingSlotIndex);
        StageUIManager.Instance.RefreshUnitSlot(slotIndex);

        StageUIManager.Instance.HideDragPreview();

        isDropComplete = true;
        draggingSlotIndex = -1;
    }

    public void ActiveSlot()
    {
        
    }

    public void InActiveSlot()
    {
        
    }
}