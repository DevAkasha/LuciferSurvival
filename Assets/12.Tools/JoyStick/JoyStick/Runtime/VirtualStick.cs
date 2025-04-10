
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[ExecuteAlways]
public class VirtualStick : Singleton<VirtualStick>, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected override bool IsPersistent => false;

    [SerializeField] private bool hide;
    [SerializeField] private bool isTouchPosition;
    [SerializeField] private Vector2 size = new Vector2(200, 200);
    [SerializeField] private RectTransform handle;
    [SerializeField] private RectTransform rt;
    [SerializeField, Range(10f, 150f)] private float handleRange;

    public Vector2 inputVector
    {
        get
        {
            return _inputVector;
        }
        private set
        {
            if (_inputVector != value)
            {
                OnHandleChanged?.Invoke(value);
            }
            _inputVector = value;
        }
    }
    [SerializeField] private Vector2 _inputVector;
    private bool isInput;

    public UnityAction<Vector2> OnHandleChanged;
    public Image safeArea;
    private Vector2 originPos;

    void OnValidate()
    {
        var nowSize = rt.sizeDelta.x;
        var changed = Mathf.Clamp(nowSize != size.x ? size.x : size.y, 100, 500);
        size = new Vector2(changed, changed);
        rt.sizeDelta = size;
        handle.sizeDelta = size / 2;
        rt.gameObject.SetActive(!hide);
    }

    protected override void Awake()
    {
        base.Awake();
        var size = handle.sizeDelta.x;
        var rtSize = rt.sizeDelta.x;
        handleRange = (rtSize - size) / 2;

        rt.gameObject.SetActive(!hide);
        originPos = rt.anchoredPosition;
    }

    public void ControlStickHandle(PointerEventData eventData)
    {
        var inputDir = eventData.position - rt.anchoredPosition - (safeArea != null ? safeArea.gameObject.transform.position : Vector2.zero);
        var clampedDir = inputDir.magnitude < handleRange ? inputDir
            : inputDir.normalized * handleRange;
        handle.anchoredPosition = clampedDir;// + rt.anchoredPosition;
        inputVector = clampedDir / handleRange;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isInput = true;
        if (isTouchPosition)
        {
            rt.position = eventData.position;
            rt.gameObject.SetActive(true);
        }
        else
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, eventData.position))
            {
                rt.gameObject.SetActive(true);
            }
            else
            {
                isInput = false;
            }
        }
        if (isInput)
            ControlStickHandle(eventData);
    }

    // 오브젝트를 클릭해서 드래그 하는 도중에 들어오는 이벤트
    // 하지만 클릭을 유지한 상태로 마우스를 멈추면 이벤트가 들어오지 않음    
    public void OnDrag(PointerEventData eventData)
    {
        if (isInput)
            ControlStickHandle(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
        rt.gameObject.SetActive(!hide);
        rt.anchoredPosition = originPos;
        isInput = false;
    }
}