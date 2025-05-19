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
    [SerializeField] private Color focusColor = Color.yellow; // 포커스 색상
    [SerializeField] private Color[] OrigenColor; // 기존색상 캐싱

    private Material[] materials;

    private void Start()
    {
        resourceRenderer = GetComponent<MeshRenderer>();
        materials = resourceRenderer.materials; // 언박싱 없이 그대로 값을 넣음
        OrigenColor = new Color[materials.Length]; // 초기화를 함. 인덱스 갯수를 정해줌.

        for (int i = 0; i < materials.Length; i++)
        {
            OrigenColor[i] = materials[i].color;// 언박싱을 함 초기화 없이 언박싱하면 인덱스에러 남
        }
    }

    public override void OnFocus()
    {
        if (!isInteractable || isFocused|| isGathered) return;
        isFocused = true;
        ChangeAllMaterialsColor(focusColor);
    }

    public override void OnLoseFocus()
    {
        if (!isFocused|| isGathered) return;
        isFocused = false;
        ChangeAllMaterialsColor(OrigenColor);
    }

    public override void Interact(PlayerEntity player)
    {
        if (isGathered)
        {
            Debug.Log("이미 채취한 자원입니다.");
            return;
        }
        Debug.Log("채취전 영혼석: " + StageManager.Instance.SoulStone);
        GatherTile();
        Debug.Log("채취후 영혼석: " + StageManager.Instance.SoulStone);
    }

    public void GatherTile()
    {
        isGathered = true;
        
        // 모든 머테리얼의 색상 변경
        ChangeAllMaterialsColor(gatheredColor);
        
        RewardManager.Instance.TryGatherResource();
    }
    
    private void ChangeAllMaterialsColor(Color color)
    {    
        if (resourceRenderer != null)
        {
            // 각 머테리얼의 색상 변경
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = color;
            }
            
            // 변경된 머테리얼 배열을 다시 적용
            resourceRenderer.materials = materials;
        }
        else
        {
            Debug.LogWarning("MeshRenderer를 찾을 수 없습니다.");
        }
    }

    private void ChangeAllMaterialsColor(Color[] color)
    {
        if (resourceRenderer != null)
        {
            // 각 머테리얼의 색상 변경
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = OrigenColor[i];
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
