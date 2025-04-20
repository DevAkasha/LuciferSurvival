
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListItem : MonoAutoCaching
{
    /// <summary>
    /// 현재 UI상 순서에 해당하는 값
    /// </summary>
    public int Index { get => transform.GetSiblingIndex(); }

    public RectTransform RectTransform { get => transform as RectTransform; }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

}
