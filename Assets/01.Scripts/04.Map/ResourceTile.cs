using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTile : BaseInteractable
{
    [SerializeField] private Renderer resourceRenderer; // Renderer 컴포넌트 참조
    [SerializeField] private bool isGathered = false; // 이미 채취했는지 여부
    
    public override void Interact(PlayerEntity player)
    {
        if (isGathered)
        {
            Debug.Log("이미 채취한 자원입니다.");
            return;
        }
        
        GatherTile();
    }

    public void GatherTile()
    {
        isGathered = true; // 채취 상태로 변경
        RewardManager.Instance.TryGatherResource();
        
        if (resourceRenderer != null)
        {
            // 해당 렌더러 안에있는 메테리얼의 배열을 선언해주고 갯수에 맞춰서 바꿔주고 해야함
            // 이건 확인해보고 잘 써보기
            // Material[] materials =
            resourceRenderer.material.SetColor("_Color", Color.gray); // 또는 원하는 색
        }
    }
}
