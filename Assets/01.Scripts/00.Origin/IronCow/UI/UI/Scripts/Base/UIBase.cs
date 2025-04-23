

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class UIOptions
{
    public bool isActiveOnLoad = true;
    public bool isDestroyOnHide = true;
    public bool isMultiple = false;
}

#if USE_AUTO_CACHING
public abstract class UIBase : MonoAutoCaching
#else
public abstract class UIBase : MonoBehaviour
#endif
{
    public UIPosition uiPosition;
    public UIOptions uiOptions;
    public UnityAction<object[]> opened;
    public UnityAction<object[]> closed;
    public RectTransform rectTransform { get => transform as RectTransform; }

    protected virtual void Awake()
    {
        opened = Opened;
        closed = Closed;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public abstract void HideDirect();

    public abstract void Opened(object[] param);

    public virtual void Closed(object[] param) { }

#if USE_AUTO_CACHING && UNITY_EDITOR
        protected override void OnValidate()
        { 
            base.OnValidate();
#else
    void OnValidate()
    {
#endif
        SetUIType();
    }

    public void SetUIType()
    {
        if (name.Contains("Gnb"))
        {
            uiPosition = UIPosition.GNB;
        }
        else if (name.Contains("Top"))
        {
            uiPosition = UIPosition.Top;
        }
        else if (name.Contains("Popup"))
        {
            uiPosition = UIPosition.Popup;
        }
        else if (name.Contains("UI"))
        {
            uiPosition = UIPosition.UI;
        }
    }
}