using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프리펩에 붙일 스크립트
public class LockTile : BaseInteractable
{
    [SerializeField] private int unlockCost = 5; // 기본값 5로 설정, Inspector에서 조정 가능

    public override void Interact(PlayerEntity player)
    {
        if (StageManager.Instance.UseSoulStone(unlockCost))
        {
            // 재화가 충분하면 타일 제거
            RemoveTile();
        }
        else
        {
            Debug.Log("영혼석 부족");
        }
    }

    // 파괴
    public void RemoveTile()
    {
        Destroy(gameObject);
    }
}
