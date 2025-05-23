using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        int idx = UnitSlot.draggingSlotIndex;
        if (idx < 0)
        {
            Debug.Log("[SellArea] 드래그 중인 슬롯이 없습니다.");
            return;
        }

        bool sold = UnitManager.Instance.SellUnit(idx);
        if (!sold)
        {
            Debug.Log("[SellArea] 판매 실패: 슬롯이 비었거나 인덱스 오류");
        }

        // 3) 드래그 성공 플래그 & 인덱스 초기화
        UnitSlot.isDropComplete = true;
        UnitSlot.draggingSlotIndex = -1;
    }
}