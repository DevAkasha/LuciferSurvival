using System;
using System.Collections.Generic;

public sealed class RxModLong : IRxMod<long>, IModifiable, IUntypedRxMod
{
    private long origin;
    private long cachedValue;
    private long lastNotifiedValue;
    private bool dirty = true;

    private readonly Dictionary<ModifierKey, long> additives = new();
    private readonly Dictionary<ModifierKey, double> additiveMultipliers = new(); // long에는 double 비율 적용
    private readonly Dictionary<ModifierKey, double> multipliers = new();
    private readonly Dictionary<ModifierKey, long> postMultiplicativeAdditives = new();
    private readonly HashSet<ModifierKey> signModifiers = new();

    public event Action<long> OnChanged;

    public RxModLong(long origin = 0)
    {
        this.origin = origin;
        this.cachedValue = origin;
        this.lastNotifiedValue = origin;
    }

    public long Value
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
        long sum = origin;
        foreach (var v in additives.Values)
            sum += v;

        double additiveRate = 0;
        foreach (var v in additiveMultipliers.Values)
            additiveRate += v;

        double scaled = sum * (1 + additiveRate);

        double mul = 1;
        foreach (var v in multipliers.Values)
            mul *= v;

        double withMul = scaled * mul;

        long postAdd = 0;
        foreach (var v in postMultiplicativeAdditives.Values)
            postAdd += v;

        long total = (long)Math.Round(withMul + postAdd);

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

    public void SetOrigin(long value)
    {
        origin = value;
        Invalidate();
    }

    public void SetValue(long newOrigin)
    {
        origin = newOrigin;
        Invalidate();
        ForceUpdate();
    }

    public void ResetValue(long newValue)
    {
        origin = newValue;
        ClearAll();
        cachedValue = newValue;
        lastNotifiedValue = newValue;
        OnChanged?.Invoke(newValue);
    }

    public void SetModifier(ModifierType type, ModifierKey key, long value)
    {
        switch (type)
        {
            case ModifierType.OriginAdd:
                additives[key] = value;
                break;
            case ModifierType.AddMultiplier:
                additiveMultipliers[key] = value / 100.0; // 퍼센트 개념
                break;
            case ModifierType.Multiplier:
                multipliers[key] = value / 100.0;
                break;
            case ModifierType.FinalAdd:
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
            ModifierType.OriginAdd => additives.Remove(key),
            ModifierType.AddMultiplier => additiveMultipliers.Remove(key),
            ModifierType.Multiplier => multipliers.Remove(key),
            ModifierType.FinalAdd => postMultiplicativeAdditives.Remove(key),
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

    // === IRxMod<long> 명시적 구현 ===
    long IRxMod<long>.Value => Value;
    void IRxMod<long>.SetValue(long origin) => SetValue(origin);
    void IRxMod<long>.ResetValue(long origin) => ResetValue(origin);
    void IRxMod<long>.SetModifier(ModifierType type, ModifierKey key, long value) => SetModifier(type, key, value);
    void IRxMod<long>.AddModifier(ModifierType type, ModifierKey key) => AddModifier(type, key);
    void IRxMod<long>.RemoveModifier(ModifierType type, ModifierKey key) => RemoveModifier(type, key);
    void IRxMod<long>.ClearAll() => ClearAll();

    // === IUntypedRxMod 명시적 구현 ===
    object IUntypedRxMod.Value => Value;
    void IUntypedRxMod.SetValue(object origin)
    {
        if (origin is long value) SetValue(value);
        else throw new InvalidCastException("Expected long");
    }

    void IUntypedRxMod.ResetValue(object origin)
    {
        if (origin is long value) ResetValue(value);
        else throw new InvalidCastException("Expected long");
    }

    void IUntypedRxMod.SetModifier(ModifierType type, ModifierKey key, object value)
    {
        if (value is long l) SetModifier(type, key, l);
        else throw new InvalidCastException("Expected long");
    }

    void IUntypedRxMod.AddModifier(ModifierType type, ModifierKey key) => AddModifier(type, key);
    void IUntypedRxMod.RemoveModifier(ModifierType type, ModifierKey key) => RemoveModifier(type, key);
    void IUntypedRxMod.ClearAll() => ClearAll();

    // === IModifiable 구현 ===
    public void ApplyModifier(ModifierKey key, ModifierType type, object value)
    {
        if (value is long l)
            SetModifier(type, key, l);
        else
            throw new InvalidCastException("IModifiable requires value of type long");
    }

    public void ApplySignFlip(ModifierKey key)
        => AddModifier(ModifierType.SignFlip, key);

    public void RemoveModifier(ModifierKey key)
    {
        RemoveModifier(ModifierType.OriginAdd, key);
        RemoveModifier(ModifierType.AddMultiplier, key);
        RemoveModifier(ModifierType.Multiplier, key);
        RemoveModifier(ModifierType.FinalAdd, key);
        RemoveModifier(ModifierType.SignFlip, key);
    }
}