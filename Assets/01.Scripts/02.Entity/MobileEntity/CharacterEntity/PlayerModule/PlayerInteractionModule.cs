using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerInteractionModule : PlayerPart
{
    [Header("상호작용 설정")]
    [SerializeField] private float interactionRadius = 3f;
    [SerializeField] private LayerMask interactableLayers; // 여러 레이어를 선택할 수 있는 LayerMask
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject interactionPromptUI;

    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private IInteractable currentFocusedInteractable;

    protected override void AtInit()
    {
        base.AtInit();

        if (interactionPromptUI != null)
            interactionPromptUI.SetActive(false);
    }

    private void Update()
    {
        DetectInteractables();
        UpdateFocus();

        // 키보드 입력 검사
        if (Input.GetKeyDown(interactionKey) && currentFocusedInteractable != null)
        {
            currentFocusedInteractable.Interact(Entity);
        }
    }

    private void DetectInteractables()
    {
        interactablesInRange.Clear();

        if (TimeManager.Instance.currentTimeState == TimeState.Day) return;

        // 여러 레이어에서 상호작용 가능한 오브젝트 검색
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayers);

        foreach (var collider in colliders)
        {
            // 오브젝트 또는 자식 오브젝트에서 IInteractable 검색
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable == null)
            {
                // 컴포넌트가 부모 오브젝트에 있을 수도 있음
                interactable = collider.GetComponentInParent<IInteractable>();
            }

            if (interactable != null && interactable.IsInteractable && !interactablesInRange.Contains(interactable))
            {
                interactablesInRange.Add(interactable);
            }
        }
    }

    private void UpdateFocus()
    {
        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (var interactable in interactablesInRange)
        {
            float distance = interactable.GetDistance(transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        // UI 프롬프트 표시 여부 결정
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(closestInteractable != null);
        }

        // 이전 포커스와 다르면 변경
        if (currentFocusedInteractable != closestInteractable)
        {
            // 이전 포커스 해제
            if (currentFocusedInteractable != null)
                currentFocusedInteractable.OnLoseFocus();

            // 새로운 포커스 설정
            currentFocusedInteractable = closestInteractable;
            if (currentFocusedInteractable != null)
                currentFocusedInteractable.OnFocus();
        }
    }

    // InputSystem으로 상호작용 버튼 입력 처리
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.performed && currentFocusedInteractable != null)
        {
            currentFocusedInteractable.Interact(Entity);
        }
    }

    // 에디터에서 디버깅을 위한 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}