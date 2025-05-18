using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIBase : MonoBehaviour
{
    public UnityAction<object[]> opened;
    public UnityAction<object[]> closed;

    protected virtual void Awake()
    {
        opened = Opened;
        closed = Closed;
    }

    //UI가 열릴때 할 일(UI 열릴때 초기화)
    public abstract void Opened(object[] param);

    //UI가 닫힐때 할 일(구현할 필요가 없으면 구현하지 않아도 됨)
    public virtual void Closed(object[] param) { }

    public abstract void HideDirect();
}
