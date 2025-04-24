using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프리펩에 붙일 스크립트
public class LockTile : MonoBehaviour
{
    // 파괴
    public void RemoveTile()
    {
        Destroy(this);
    }
}
