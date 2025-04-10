using System.Collections.Generic;
using System;

public sealed class RxModInt : RxBase, IRxMod<int>, IModifiable, IUntypedRxMod
{
    private int origin;
    private int cachedValue;
    private int lastNotifiedValue;
    private bool dirty = true;

    private readonly Dictionary<ModifierKey, int> additives = new();
    private readonly Dictionary<ModifierKey, float> additiveMultipliers = new();
    private readonly Dictionary<ModifierKey, float> multipliers = new();
    private readonly Dictionary<ModifierKey, int> postMultiplicativeAdditives = new();
    private readonly HashSet<ModifierKey> signModifiers = new();

    private readonly List<Action<int>> listeners = new();

    public RxModInt(int origin = 0, object owner = null)
    {
        this.origin = origin;
        cachedValue = origin;
        lastNotifiedValue = origin;
        if (owner is ITrackableRxModel model)
        {
            model.RegisterRx(this);
        }
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

    public void AddListener(Action<int> listener)
    {
        if (listener != null)
        {
            listeners.Add(listener);
            listener(Value);
        }
    }

    public void RemoveListener(Action<int> listener)
    {
        listeners.Remove(listener);
    }

    private void Recalculate()
    {
        float sum = origin;
        foreach (var v in additives.Values)
            sum += v;

        float additiveRate = 0f;
        foreach (var v in additiveMultipliers.Values)
            additiveRate += v;

        float scaled = sum * (1f + additiveRate);

        float mul = 1f;
        foreach (var v in multipliers.Values)
            mul *= v;

        float withMul = scaled * mul;

        float postAdd = 0f;
        foreach (var v in postMultiplicativeAdditives.Values)
            postAdd += v;

        float total = withMul + postAdd;

        if (signModifiers.Count % 2 != 0)
            total = -total;

        cachedValue = (int)Math.Round(total);

        if (cachedValue != lastNotifiedValue)
        {
            NotifyAll(cachedValue);
            lastNotifiedValue = cachedValue;
        }

        dirty = false;
    }

    private void NotifyAll(int value)
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
    }

    public void SetModifier(ModifierType type, ModifierKey key, float value)
    {
        switch (type)
        {
            case ModifierType.OriginAdd:
                additives[key] = (int)value;
                break;
            case ModifierType.AddMultiplier:
                additiveMultipliers[key] = value;
                break;
            case ModifierType.Multiplier:
                multipliers[key] = value;
                break;
            case ModifierType.FinalAdd:
                postMultiplicativeAdditives[key] = (int)value;
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

    // === IRxMod<int> 명시적 구현 ===
    int IRxMod<int>.Value => Value;
    void IRxMod<int>.SetValue(int origin) => SetValue(origin);
    void IRxMod<int>.ResetValue(int origin) => ResetValue(origin);
    void IRxMod<int>.SetModifier(ModifierType type, ModifierKey key, int value) => SetModifier(type, key, value);
    void IRxMod<int>.AddModifier(ModifierType type, ModifierKey key) => AddModifier(type, key);
    void IRxMod<int>.RemoveModifier(ModifierType type, ModifierKey key) => RemoveModifier(type, key);
    void IRxMod<int>.ClearAll() => ClearAll();

    // === IUntypedRxMod 명시적 구현 ===
    object IUntypedRxMod.Value => Value;
    void IUntypedRxMod.SetValue(object origin)
    {
        if (origin is int value) SetValue(value);
        else throw new InvalidCastException("Expected int");
    }

    void IUntypedRxMod.ResetValue(object origin)
    {
        if (origin is int value) ResetValue(value);
        else throw new InvalidCastException("Expected int");
    }

    void IUntypedRxMod.SetModifier(ModifierType type, ModifierKey key, object value)
    {
        if (value is float f) SetModifier(type, key, f);
        else throw new InvalidCastException("Expected float");
    }

    void IUntypedRxMod.AddModifier(ModifierType type, ModifierKey key) => AddModifier(type, key);
    void IUntypedRxMod.RemoveModifier(ModifierType type, ModifierKey key) => RemoveModifier(type, key);
    void IUntypedRxMod.ClearAll() => ClearAll();

    // === IModifiable 구현 ===
    public void ApplyModifier(ModifierKey key, ModifierType type, object value)
    {
        if (value is float f)
            SetModifier(type, key, f);
        else
            throw new InvalidCastException("IModifiable requires value of type float");
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