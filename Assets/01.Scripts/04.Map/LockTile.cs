using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프리펩에 붙일 스크립트
public class LockTile : BaseInteractable
{
    public override void Interact(PlayerEntity player)
    {
        RemoveTile();
    }

    // 파괴
    public void RemoveTile()
    {
        Destroy(gameObject);
    }
}
