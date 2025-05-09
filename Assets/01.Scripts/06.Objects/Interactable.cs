using System;
using UnityEngine;

// 상호작용 인터페이스
public interface IInteractable
{
    bool IsInteractable { get; }
    float GetDistance(Vector3 position);
    void OnFocus();
    void OnLoseFocus();
    void Interact(PlayerEntity player);
}

// 상호작용 기본 구현 클래스
public abstract class BaseInteractable : WorldObject, IInteractable
{
    [Header("상호작용 설정")]
    [SerializeField] protected bool isInteractable = true;

    [Header("아웃라인 설정")]
    [SerializeField] protected Renderer targetRenderer;
    [SerializeField] protected Color outlineColor = Color.yellow;
    [SerializeField] protected string emissionColorProperty = "_EmissionColor";
    [SerializeField] protected float emissionIntensity = 2f;

    private bool isFocused = false;
    private Color originalEmissionColor;
    private bool wasEmissionEnabled = false;

    public bool IsInteractable => isInteractable;

    protected void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            // 머티리얼 인스턴스 생성 (공유 머티리얼을 수정하지 않도록)
            targetRenderer.material = new Material(targetRenderer.material);

            // 원래 Emission 상태 저장
            wasEmissionEnabled = targetRenderer.material.IsKeywordEnabled("_EMISSION");

            // 원래 Emission 색상 저장 (있는 경우)
            if (targetRenderer.material.HasProperty(emissionColorProperty))
            {
                originalEmissionColor = targetRenderer.material.GetColor(emissionColorProperty);
            }
            else
            {
                originalEmissionColor = Color.black;
            }
        }
    }

    public float GetDistance(Vector3 position)
    {
        return Vector3.Distance(transform.position, position);
    }

    public void OnFocus()
    {
        if (!isInteractable || isFocused) return;

        isFocused = true;
        if (targetRenderer != null)
        {
            // Emission 활성화
            targetRenderer.material.EnableKeyword("_EMISSION");

            // Emission 색상 설정
            if (targetRenderer.material.HasProperty(emissionColorProperty))
            {
                targetRenderer.material.SetColor(emissionColorProperty, outlineColor * emissionIntensity);
            }

            // URP에서는 이 속성도 설정해야 할 수 있음
            if (targetRenderer.material.HasProperty("_EmissiveColor"))
            {
                targetRenderer.material.SetColor("_EmissiveColor", outlineColor * emissionIntensity);
            }

            // Emission 강도 설정 (URP에서는 다른 방식으로 설정해야 할 수 있음)
            if (targetRenderer.material.HasProperty("_EmissionIntensity"))
            {
                targetRenderer.material.SetFloat("_EmissionIntensity", emissionIntensity);
            }
        }
    }

    public void OnLoseFocus()
    {
        if (!isFocused) return;

        isFocused = false;
        if (targetRenderer != null)
        {
            // 원래 상태로 복원
            if (!wasEmissionEnabled)
            {
                targetRenderer.material.DisableKeyword("_EMISSION");
            }
            else
            {
                // 원래 Emission 색상으로 복원
                if (targetRenderer.material.HasProperty(emissionColorProperty))
                {
                    targetRenderer.material.SetColor(emissionColorProperty, originalEmissionColor);
                }

                // URP에서는 이 속성도 복원
                if (targetRenderer.material.HasProperty("_EmissiveColor"))
                {
                    targetRenderer.material.SetColor("_EmissiveColor", originalEmissionColor);
                }
            }
        }
    }

    public abstract void Interact(PlayerEntity player);

    public void SetInteractable(bool value)
    {
        isInteractable = value;
        if (!isInteractable && isFocused)
            OnLoseFocus();
    }

    protected void OnDestroy()
    {
        // 머티리얼 정리
        if (targetRenderer != null && targetRenderer.material != null)
        {
            Destroy(targetRenderer.material);
        }
    }
}
