using System;
using System.Collections.Generic;

public sealed class RxModInt : RxModBase<int>
{
    private readonly Dictionary<ModifierKey, int> additives = new();
    private readonly Dictionary<ModifierKey, float> additiveMultipliers = new();
    private readonly Dictionary<ModifierKey, float> multipliers = new();
    private readonly Dictionary<ModifierKey, int> postMultiplicativeAdditives = new();
    private readonly HashSet<ModifierKey> signModifiers = new();

    public RxModInt(int origin = 0, string fieldName = null, object owner = null)
    {
        this.origin = origin;
        cachedValue = origin;
        lastNotifiedValue = origin;

        if (!string.IsNullOrEmpty(fieldName))
            FieldName = fieldName;

        if (owner is ITrackableRxModel model)
            model.RegisterRx(this);
    }

    protected override void Recalculate()
    {
        float sum = origin;
        foreach (var v in additives.Values) sum += v;

        float additiveRate = 0f;
        foreach (var v in additiveMultipliers.Values) additiveRate += v;

        float scaled = sum * (1f + additiveRate);

        float mul = 1f;
        foreach (var v in multipliers.Values) mul *= v;

        float withMul = scaled * mul;

        float postAdd = 0f;
        foreach (var v in postMultiplicativeAdditives.Values) postAdd += v;

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

    public override void ClearAll()
    {
        additives.Clear();
        additiveMultipliers.Clear();
        multipliers.Clear();
        postMultiplicativeAdditives.Clear();
        signModifiers.Clear();
        Invalidate();
    }

    public override void SetModifier(ModifierType type, ModifierKey key, int value)
    {
        switch (type)
        {
            case ModifierType.OriginAdd: additives[key] = value; break;
            case ModifierType.AddMultiplier: additiveMultipliers[key] = value; break;
            case ModifierType.Multiplier: multipliers[key] = value; break;
            case ModifierType.FinalAdd: postMultiplicativeAdditives[key] = value; break;
            default: throw new InvalidOperationException("Use AddModifier for SignFlip.");
        }
        Invalidate();
    }

    public override void AddModifier(ModifierType type, ModifierKey key)
    {
        if (type != ModifierType.SignFlip)
            throw new InvalidOperationException("Only SignFlip can be added without a value.");
        signModifiers.Add(key);
        Invalidate();
    }

    public override void RemoveModifier(ModifierType type, ModifierKey key)
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
        if (removed) Invalidate();
    }
}