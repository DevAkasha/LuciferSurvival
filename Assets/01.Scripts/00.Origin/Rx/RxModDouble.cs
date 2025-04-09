using System;
using System.Collections.Generic;

public sealed class RxModDouble : IRxMod<double>, IUntypedRxMod
{
    private double origin;
    private double cachedValue;
    private double lastNotifiedValue;
    private bool dirty = true;

    private readonly Dictionary<ModifierKey, double> additives = new();
    private readonly Dictionary<ModifierKey, double> additiveMultipliers = new();
    private readonly Dictionary<ModifierKey, double> multipliers = new();
    private readonly Dictionary<ModifierKey, double> postMultiplicativeAdditives = new();
    private readonly HashSet<ModifierKey> signModifiers = new();

    public event Action<double> OnChanged;

    public RxModDouble(double origin = 0.0)
    {
        this.origin = origin;
        this.cachedValue = origin;
        this.lastNotifiedValue = origin;
    }

    public double Value
    {
        get
        {
            if (dirty)
                Recalculate();

            return cachedValue;
        }
    }

    private void Recalculate()
    {
        double sum = origin;
        foreach (var v in additives.Values)
            sum += v;

        double additiveRate = 0.0;
        foreach (var v in additiveMultipliers.Values)
            additiveRate += v;

        double scaled = sum * (1.0 + additiveRate);

        double mul = 1.0;
        foreach (var v in multipliers.Values)
            mul *= v;

        double withMul = scaled * mul;

        double postAdd = 0.0;
        foreach (var v in postMultiplicativeAdditives.Values)
            postAdd += v;

        double total = withMul + postAdd;

        if (signModifiers.Count % 2 != 0)
            total = -total;

        cachedValue = total;

        if (!cachedValue.Equals(lastNotifiedValue))
        {
            lastNotifiedValue = cachedValue;
            OnChanged?.Invoke(cachedValue);
        }

        dirty = false;
    }

    private void Invalidate() => dirty = true;

    public void ForceUpdate()
    {
        Invalidate();
        _ = Value;
    }

    public void SetOrigin(double value)
    {
        origin = value;
        Invalidate();
    }

    public void SetValue(double newOrigin)
    {
        origin = newOrigin;
        Invalidate();
        ForceUpdate();
    }

    public void ResetValue(double newValue)
    {
        origin = newValue;
        ClearAll();
        cachedValue = newValue;
        lastNotifiedValue = newValue;
        OnChanged?.Invoke(newValue);
    }

    public void SetModifier(ModifierType type, ModifierKey key, double value)
    {
        switch (type)
        {
            case ModifierType.Additive:
                additives[key] = value;
                break;
            case ModifierType.AdditiveMultiplier:
                additiveMultipliers[key] = value;
                break;
            case ModifierType.Multiplier:
                multipliers[key] = value;
                break;
            case ModifierType.PostMultiplicativeAdditive:
                postMultiplicativeAdditives[key] = value;
                break;
            default:
                throw new InvalidOperationException("Use AddModifier for SignFlip.");
        }

        Invalidate();
    }

    public void AddModifier(ModifierType type, ModifierKey key)
    {
        if (type != ModifierType.SignFlip)
            throw new InvalidOperationException("Only SignFlip can be added without a value.");

        signModifiers.Add(key);
        Invalidate();
    }

    public void RemoveModifier(ModifierType type, ModifierKey key)
    {
        bool removed = type switch
        {
            ModifierType.Additive => additives.Remove(key),
            ModifierType.AdditiveMultiplier => additiveMultipliers.Remove(key),
            ModifierType.Multiplier => multipliers.Remove(key),
            ModifierType.PostMultiplicativeAdditive => postMultiplicativeAdditives.Remove(key),
            ModifierType.SignFlip => signModifiers.Remove(key),
            _ => false
        };

        if (removed)
            Invalidate();
    }

    public void ClearAll()
    {
        additives.Clear();
        additiveMultipliers.Clear();
        multipliers.Clear();
        postMultiplicativeAdditives.Clear();
        signModifiers.Clear();
        Invalidate();
    }

    double IRxMod<double>.Value => Value;

    void IRxMod<double>.SetValue(double origin) => SetValue(origin);

    void IRxMod<double>.ResetValue(double origin) => ResetValue(origin);

    void IRxMod<double>.SetModifier(ModifierType type, ModifierKey key, double value) =>
        SetModifier(type, key, value);

    void IRxMod<double>.AddModifier(ModifierType type, ModifierKey key) =>
        AddModifier(type, key);

    void IRxMod<double>.RemoveModifier(ModifierType type, ModifierKey key) =>
        RemoveModifier(type, key);

    void IRxMod<double>.ClearAll() => ClearAll();

    object IUntypedRxMod.Value => Value;

    void IUntypedRxMod.SetValue(object origin)
    {
        if (origin is double value)
            SetValue(value);
        else
            throw new InvalidCastException("Expected double");
    }

    void IUntypedRxMod.ResetValue(object origin)
    {
        if (origin is double value)
            ResetValue(value);
        else
            throw new InvalidCastException("Expected double");
    }

    void IUntypedRxMod.SetModifier(ModifierType type, ModifierKey key, object value)
    {
        if (value is double v)
            SetModifier(type, key, v);
        else
            throw new InvalidCastException("Expected double");
    }

    void IUntypedRxMod.AddModifier(ModifierType type, ModifierKey key) =>
        AddModifier(type, key);

    void IUntypedRxMod.RemoveModifier(ModifierType type, ModifierKey key) =>
        RemoveModifier(type, key);

    void IUntypedRxMod.ClearAll() => ClearAll();
}