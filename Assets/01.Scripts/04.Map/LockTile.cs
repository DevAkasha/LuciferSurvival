using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프리펩에 붙일 스크립트
public class LockTile : BaseInteractable
{
    [SerializeField] private int unlockCost = 5; // 기본값 5로 설정, Cost부분은 나중에 수정 혹은 삭제예정

    public override void Interact(PlayerEntity player)
    {
        // 영혼석이 아닌 영혼핵으로 변경하기 - 몬스터 처치 시 획득하는 재화

        //if (StageManager.Instance.UseSoulStone(unlockCost))
        //{
        //    RemoveTile();
        //}
        //else
        //{
        //    Debug.Log("영혼석 부족");
        //}
    }

    // 파괴
    public void RemoveTile()
    {
        Destroy(gameObject);
    }
}
