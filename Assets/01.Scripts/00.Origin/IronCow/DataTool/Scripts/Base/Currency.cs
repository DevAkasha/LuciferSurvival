
using System;
using System.Collections.Generic;
using UnityEngine;

public class Currency32
{
    public int value;
    Action<int> action;

    public void Subscribe(Action<int> action)
    {
        this.action += action;
    }

    public void Unsubscribe(Action<int> action)
    {
        this.action -= action;
    }

    public Currency32 AddValue(int value)
    {
        this.value += value;
        return this;
    }

    public static Currency32 operator +(Currency32 value1, int value2)
    {
        var currencies = value1.AddValue(value2);
        currencies.action.Invoke(currencies.value);
        return currencies;
    }

    public static Currency32 operator -(Currency32 value1, int value2)
    {
        var currencies = value1.AddValue(-value2);
        currencies.action.Invoke(currencies.value);
        return currencies;
    }
}


public class Currency64
{
    public long value;
    Action<long> action;

    public void Subscribe(Action<long> action)
    {
        this.action += action;
    }

    public void Unsubscribe(Action<long> action)
    {
        this.action -= action;
    }

    public Currency64 AddValue(long value)
    {
        this.value += value;
        return this;
    }

    public static Currency64 operator +(Currency64 value1, long value2)
    {
        var currencies = value1.AddValue(value2);
        currencies.action.Invoke(currencies.value);
        return currencies;
    }

    public static Currency64 operator -(Currency64 value1, long value2)
    {
        var currencies = value1.AddValue(-value2);
        currencies.action.Invoke(currencies.value);
        return currencies;
    }
}
