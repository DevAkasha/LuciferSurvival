using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTile : BaseInteractable
{
    [SerializeField] private MeshRenderer resourceRenderer; // Renderer 컴포넌트 참조
    [SerializeField] private Texture2D grayTexture; // 회색 텍스처를 인스펙터에서 할당
    [SerializeField] private bool isGathered = false; // 이미 채취했는지 여부
    [SerializeField] private Color gatheredColor = Color.gray; // 채취 후 색상

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
        isGathered = true;
        
        // 모든 머테리얼의 색상 변경
        ChangeAllMaterialsColor();
        
        RewardManager.Instance.TryGatherResource();
    }
    
    private void ChangeAllMaterialsColor()
    {
        // resourceRenderer가 할당되지 않았다면 현재 게임 오브젝트에서 가져옴
        if (resourceRenderer == null)
        {
            resourceRenderer = GetComponent<MeshRenderer>();
        }
        
        if (resourceRenderer != null)
        {
            // 모든 머테리얼 가져오기
            Material[] materials = resourceRenderer.materials;
            
            // 각 머테리얼의 색상 변경
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = gatheredColor;
                
                // 텍스처가 할당되어 있다면 텍스처도 변경 가능
                if (grayTexture != null)
                {
                    materials[i].mainTexture = grayTexture;
                }
            }
            
            // 변경된 머테리얼 배열을 다시 적용
            resourceRenderer.materials = materials;
        }
        else
        {
            Debug.LogWarning("MeshRenderer를 찾을 수 없습니다.");
        }
    }
}
