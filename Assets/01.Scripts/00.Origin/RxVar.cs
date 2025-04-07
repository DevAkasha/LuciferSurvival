using System.Collections.Generic;
using System;

public class RxVar<T>
{
    private T _value;
    private readonly List<Action<T>> _listeners = new();

    public RxVar(T initialValue = default)
    {
        _value = initialValue;
    }

    public T Value => _value;

    public void SetValue(T newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(_value, newValue))
        {
            _value = newValue;
            NotifyAll();
        }
    }

    public void AddListener(Action<T> listener)
    {
        if (listener != null)
        {
            _listeners.Add(listener);
            listener(_value); 
        }
    }

    public void RemoveListener(Action<T> listener)
    {
        _listeners.Remove(listener);
    }

    private void NotifyAll()
    {
        foreach (var listener in _listeners)
            listener(_value);
    }
}