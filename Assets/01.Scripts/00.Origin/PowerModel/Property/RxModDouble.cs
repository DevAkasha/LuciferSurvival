using System;
using System.Collections.Generic;
using System.Text;

public sealed class RxModDouble : RxModBase<double>, IRxModFormulaProvider
{
    private readonly Dictionary<ModifierKey, double> additives = new();
    private readonly Dictionary<ModifierKey, double> additiveMultipliers = new();
    private readonly Dictionary<ModifierKey, double> multipliers = new();
    private readonly Dictionary<ModifierKey, double> postMultiplicativeAdditives = new();
    private readonly HashSet<ModifierKey> signModifiers = new();

    private double sum, addMul, mul, postAdd;
    private bool isNegative;

    public RxModDouble(double origin = 0, string fieldName = null, object owner = null)
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
        sum = origin;
        foreach (var v in additives.Values) sum += v;

        addMul = 1.0;
        foreach (var v in additiveMultipliers.Values) addMul += v;

        mul = 1.0;
        foreach (var v in multipliers.Values) mul *= v;

        postAdd = 0.0;
        foreach (var v in postMultiplicativeAdditives.Values) postAdd += v;

        isNegative = signModifiers.Count % 2 == 1;

        double result = (sum * addMul * mul) + postAdd;
        cachedValue = isNegative ? -result : result;

        if (!cachedValue.Equals(lastNotifiedValue))
        {
            NotifyAll(cachedValue);
            lastNotifiedValue = cachedValue;
        }

        dirty = false;
    }

    protected override string BuildDebugFormula()
    {
        var sb = new StringBuilder();
        if (isNegative)
            sb.Append("-1 * ");

        sb.Append('(').Append(sum).Append(')');
        sb.Append(" * ").Append(addMul);
        sb.Append(" * ").Append(mul);
        sb.Append(" + ").Append(postAdd);

        return sb.ToString();
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

    public override void SetModifier(ModifierType type, ModifierKey key, double value)
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
