using System.Collections.Generic;
using System;

public class RxVar<T> : RxBase
{
    private T value;
    private readonly List<Action<T>> listeners = new();

    public RxVar(T initialValue = default, object owner = null)
    {
        value = initialValue;
        if (owner is ITrackableRxModel model)
        {
            model.RegisterRx(this);
        }
    }


    public T Value => value;

    public void SetValue(T newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(value, newValue))
        {
            value = newValue;
            NotifyAll();
        }
    }

    public void AddListener(Action<T> listener)
    {
        if (listener != null)
        {
            listeners.Add(listener);
            listener(value); 
        }
    }

    public void RemoveListener(Action<T> listener)
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