using System.Collections.Generic;
using System;

public sealed class RxModLong : RxBase, IRxMod<long>, IModifiable, IUntypedRxMod
{
    private long origin;
    private long cachedValue;
    private long lastNotifiedValue;
    private bool dirty = true;

    private readonly Dictionary<ModifierKey, long> additives = new();
    private readonly Dictionary<ModifierKey, double> additiveMultipliers = new();
    private readonly Dictionary<ModifierKey, double> multipliers = new();
    private readonly Dictionary<ModifierKey, long> postMultiplicativeAdditives = new();
    private readonly HashSet<ModifierKey> signModifiers = new();

    private readonly List<Action<long>> listeners = new();

    public RxModLong(long origin = 0L, object owner = null)
    {
        this.origin = origin;
        cachedValue = origin;
        lastNotifiedValue = origin;
        if (owner is ITrackableRxModel model)
        {
            model.RegisterRx(this);
        }
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

    public void AddListener(Action<long> listener)
    {
        if (listener != null)
        {
            listeners.Add(listener);
            listener(Value);
        }
    }

    public void RemoveListener(Action<long> listener)
    {
        listeners.Remove(listener);
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

        cachedValue = (long)Math.Round(total);

        if (cachedValue != lastNotifiedValue)
        {
            NotifyAll(cachedValue);
            lastNotifiedValue = cachedValue;
        }

        dirty = false;
    }

    private void NotifyAll(long value)
    {
        foreach (var listener in listeners)
            listener(value);
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
    }

    public void SetModifier(ModifierType type, ModifierKey key, double value)
    {
        switch (type)
        {
            case ModifierType.OriginAdd:
                additives[key] = (long)value;
                break;
            case ModifierType.AddMultiplier:
                additiveMultipliers[key] = value;
                break;
            case ModifierType.Multiplier:
                multipliers[key] = value;
                break;
            case ModifierType.FinalAdd:
                postMultiplicativeAdditives[key] = (long)value;
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

    public override void ClearRelation()
    {
        listeners.Clear();
        ClearAll();
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
        if (value is double d) SetModifier(type, key, d);
        else if (value is float f) SetModifier(type, key, f);
        else throw new InvalidCastException("Expected double or float");
    }

    void IUntypedRxMod.AddModifier(ModifierType type, ModifierKey key) => AddModifier(type, key);
    void IUntypedRxMod.RemoveModifier(ModifierType type, ModifierKey key) => RemoveModifier(type, key);
    void IUntypedRxMod.ClearAll() => ClearAll();

    // === IModifiable 구현 ===
    public void ApplyModifier(ModifierKey key, ModifierType type, object value)
    {
        if (value is double d)
            SetModifier(type, key, d);
        else if (value is float f)
            SetModifier(type, key, f);
        else
            throw new InvalidCastException("IModifiable requires value of type double or float");
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