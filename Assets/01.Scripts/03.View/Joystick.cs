using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public PlayerController player;

    public RectTransform joystickBackground;
    public RectTransform joystickHandle;

    public Vector2 Direction { get; private set; }

    private Vector2 inputVector;
    private Vector2 startPosition;
    private float handleRange;

    void Start()
    {
        player = PlayerManager.Instance.Player;
        startPosition = joystickBackground.position;
        handleRange = joystickBackground.sizeDelta.x * 0.5f;
    }
    private void FixedUpdate()
    {
        if(player != null)
            player.OnMove(Direction);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = eventData.position;
        Vector2 offset = position - startPosition;

        Vector2 clamped = Vector2.ClampMagnitude(offset, handleRange);
        joystickHandle.position = startPosition + clamped;

        inputVector = clamped / handleRange;
        Direction = inputVector;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        Direction = Vector2.zero;
        joystickHandle.position = startPosition;
    }
}