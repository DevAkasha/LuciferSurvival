using System;
using System.Collections.Generic;

public sealed class RxModInt : IRxMod<int>, IUntypedRxMod
{
    private int origin;
    private int cachedValue;
    private int lastNotifiedValue;
    private bool dirty = true;

    private readonly Dictionary<ModifierKey, int> additives = new();
    private readonly Dictionary<ModifierKey, int> additiveMultipliers = new();
    private readonly Dictionary<ModifierKey, int> multipliers = new();
    private readonly Dictionary<ModifierKey, int> postMultiplicativeAdditives = new();
    private readonly HashSet<ModifierKey> signModifiers = new();

    public event Action<int> OnChanged;

    public RxModInt(int origin = 0)
    {
        this.origin = origin;
        this.cachedValue = origin;
        this.lastNotifiedValue = origin;
    }

    public int Value
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
        int sum = origin;
        foreach (var v in additives.Values)
            sum += v;

        int additiveRate = 0;
        foreach (var v in additiveMultipliers.Values)
            additiveRate += v;

        int scaled = sum * (1 + additiveRate);

        int mul = 1;
        foreach (var v in multipliers.Values)
            mul *= v;

        int withMul = scaled * mul;

        int postAdd = 0;
        foreach (var v in postMultiplicativeAdditives.Values)
            postAdd += v;

        int total = withMul + postAdd;

        if (signModifiers.Count % 2 != 0)
            total = -total;

        cachedValue = total;

        if (cachedValue != lastNotifiedValue)
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

    public void SetOrigin(int value)
    {
        origin = value;
        Invalidate();
    }

    public void SetValue(int newOrigin)
    {
        origin = newOrigin;
        Invalidate();
        ForceUpdate();
    }

    public void ResetValue(int newValue)
    {
        origin = newValue;
        ClearAll();
        cachedValue = newValue;
        lastNotifiedValue = newValue;
        OnChanged?.Invoke(newValue);
    }

    public void SetModifier(ModifierType type, ModifierKey key, int value)
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
            throw new InvalidOperationException("Only SignFlip can be added without value.");

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

    object IUntypedRxMod.Value => Value;

    void IUntypedRxMod.SetValue(object origin)
    {
        if (origin is int value)
            SetValue(value);
        else
            throw new InvalidCastException("Expected int");
    }

    void IUntypedRxMod.ResetValue(object origin)
    {
        if (origin is int value)
            ResetValue(value);
        else
            throw new InvalidCastException("Expected int");
    }

    void IUntypedRxMod.SetModifier(ModifierType type, ModifierKey key, object value)
    {
        if (value is int v)
            SetModifier(type, key, v);
        else
            throw new InvalidCastException("Expected int");
    }

    void IUntypedRxMod.AddModifier(ModifierType type, ModifierKey key)
        => AddModifier(type, key);

    void IUntypedRxMod.RemoveModifier(ModifierType type, ModifierKey key)
        => RemoveModifier(type, key);

    void IUntypedRxMod.ClearAll() => ClearAll();
}