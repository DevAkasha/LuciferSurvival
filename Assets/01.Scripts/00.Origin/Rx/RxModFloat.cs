using System;
using System.Collections.Generic;

public sealed class RxModFloat : IRxMod<float>, IModifiable, IUntypedRxMod
{
    private float origin;
    private float cachedValue;
    private float lastNotifiedValue;
    private bool dirty = true;

    private readonly Dictionary<ModifierKey, float> additives = new();
    private readonly Dictionary<ModifierKey, float> additiveMultipliers = new();
    private readonly Dictionary<ModifierKey, float> multipliers = new();
    private readonly Dictionary<ModifierKey, float> postMultiplicativeAdditives = new();
    private readonly HashSet<ModifierKey> signModifiers = new();

    public event Action<float> OnChanged;

    public RxModFloat(float origin = 0f)
    {
        this.origin = origin;
        this.cachedValue = origin;
        this.lastNotifiedValue = origin;
    }

    public float Value
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

    public void SetOrigin(float value)
    {
        origin = value;
        Invalidate();
    }

    public void SetValue(float newOrigin)
    {
        origin = newOrigin;
        Invalidate();
        ForceUpdate();
    }

    public void ResetValue(float newValue)
    {
        origin = newValue;
        ClearAll();
        cachedValue = newValue;
        lastNotifiedValue = newValue;
        OnChanged?.Invoke(newValue);
    }

    public void SetModifier(ModifierType type, ModifierKey key, float value)
    {
        switch (type)
        {
            case ModifierType.OriginAdd:
                additives[key] = value;
                break;
            case ModifierType.AddMultiplier:
                additiveMultipliers[key] = value;
                break;
            case ModifierType.Multiplier:
                multipliers[key] = value;
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

    // === IRxMod<float> 명시적 구현 ===
    float IRxMod<float>.Value => Value;
    void IRxMod<float>.SetValue(float origin) => SetValue(origin);
    void IRxMod<float>.ResetValue(float origin) => ResetValue(origin);
    void IRxMod<float>.SetModifier(ModifierType type, ModifierKey key, float value) => SetModifier(type, key, value);
    void IRxMod<float>.AddModifier(ModifierType type, ModifierKey key) => AddModifier(type, key);
    void IRxMod<float>.RemoveModifier(ModifierType type, ModifierKey key) => RemoveModifier(type, key);
    void IRxMod<float>.ClearAll() => ClearAll();

    // === IUntypedRxMod 명시적 구현 ===
    object IUntypedRxMod.Value => Value;
    void IUntypedRxMod.SetValue(object origin)
    {
        if (origin is float value) SetValue(value);
        else throw new InvalidCastException("Expected float");
    }

    void IUntypedRxMod.ResetValue(object origin)
    {
        if (origin is float value) ResetValue(value);
        else throw new InvalidCastException("Expected float");
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