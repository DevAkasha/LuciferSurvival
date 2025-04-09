using UnityEngine.EventSystems;
using UnityEngine;

public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;

    [Header("플레이어 참조")]
    public PlayerController player;

    [Header("설정값")]
    public float handleRange = 100f;

    private Vector2 inputVector;
    public Vector2 Direction => inputVector;
    private bool isTouching = false;

    private Canvas canvas;
    private Camera uiCamera;

    void Start()
    {
        // 필요한 참조 자동 할당
        player = PlayerManager.Instance.Player;
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        joystickBackground.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player != null && isTouching)
            player.OnMoveByJoystick(Direction);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouching = true;
        // 터치 위치 기준으로 조이스틱 위치 설정
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            eventData.position,
            uiCamera,
            out localPoint
        );

        joystickBackground.anchoredPosition = localPoint;
        joystickHandle.anchoredPosition = Vector2.zero;
        joystickBackground.gameObject.SetActive(true);

        ProcessDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ProcessDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false;
        joystickBackground.gameObject.SetActive(false);
        joystickHandle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }

    private void ProcessDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            uiCamera,
            out localPoint
        );

        Vector2 clamped = Vector2.ClampMagnitude(localPoint, handleRange);
        joystickHandle.anchoredPosition = clamped;
        inputVector = clamped / handleRange;
    }
}