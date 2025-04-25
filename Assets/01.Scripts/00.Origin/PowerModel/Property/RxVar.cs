using System;
using System.Collections.Generic;

public sealed class RxVar<T> : RxBase, IRxReadable<T>
{
    private T value;
    private readonly List<Action<T>> listeners = new();

    public RxVar(T initialValue = default, object owner = null)
    {
        value = initialValue;
        if (owner is ITrackableRxModel model)
        {
            model.RegisterRx(this); // Rx 필드를 모델에 등록
        }
    }

    public T Value => value;

    public void SetValue(T newValue) // 값 설정
    {
        if (!EqualityComparer<T>.Default.Equals(value, newValue))
        {
            value = newValue;
            NotifyAll();
        }
    }

    public void AddListener(Action<T> listener) // 값 변경을 구독할 수 있음
    {
        if (listener != null)
        {
            listeners.Add(listener);
            listener(value);
        }
    }

    public void RemoveListener(Action<T> listener) // 구독 해제
    {
        listeners.Remove(listener);
    }

    public override void ClearRelation()
    {
        listeners.Clear();
    }

    private void NotifyAll()
    {
        foreach (var listener in listeners)
            listener(value);
    }
}