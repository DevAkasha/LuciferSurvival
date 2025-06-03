using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프리펩에 붙일 스크립트
public class LockTile : BaseInteractable
{
    [SerializeField] private int unlockCost = 10;

    public override void Interact(PlayerEntity player)
    {
        if (StageManager.Instance.ReduceSoulCore(unlockCost))
        {
            RemoveTile(); 
        }
        else
        {
            // UI 혹은 불가 시 동작할거
        }
    }

    // 파괴
    public void RemoveTile()
    {
        Destroy(gameObject);
    }
}
