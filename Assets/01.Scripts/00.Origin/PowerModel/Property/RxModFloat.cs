using System;
using System.Collections.Generic;

public sealed class RxModFloat : RxModBase<float>
{
    private readonly Dictionary<ModifierKey, float> additives = new();
    private readonly Dictionary<ModifierKey, float> additiveMultipliers = new();
    private readonly Dictionary<ModifierKey, float> multipliers = new();
    private readonly Dictionary<ModifierKey, float> postMultiplicativeAdditives = new();
    private readonly HashSet<ModifierKey> signModifiers = new();

    public RxModFloat(float origin = 0f, string fieldName = null, object owner = null) // 초기 원본 값
    {
        this.origin = origin; // 초기 원본 값
        cachedValue = origin; // 초기 원본 값
        lastNotifiedValue = origin; // 초기 원본 값

        if (!string.IsNullOrEmpty(fieldName))
            FieldName = fieldName;

        if (owner is ITrackableRxModel model)
            model.RegisterRx(this); // Rx 필드를 모델에 등록
    }

    protected override void Recalculate() // 최종 값 다시 계산
    {
        float sum = origin; // 초기 원본 값
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

        cachedValue = total; // 계산된 값 캐싱

        if (!cachedValue.Equals(lastNotifiedValue)) // 계산된 값 캐싱
        {
            NotifyAll(cachedValue); // 계산된 값 캐싱
            lastNotifiedValue = cachedValue; // 계산된 값 캐싱
        }

        dirty = false;
    }

    public override void ClearAll() // 모든 Modifier 초기화
    {
        additives.Clear();
        additiveMultipliers.Clear();
        multipliers.Clear();
        postMultiplicativeAdditives.Clear();
        signModifiers.Clear();
        Invalidate(); // 재계산 요청 (dirty 플래그 설정)
    }

    public override void SetModifier(ModifierType type, ModifierKey key, float value)
    {
        switch (type)
        {
            case ModifierType.OriginAdd: additives[key] = value; break;
            case ModifierType.AddMultiplier: additiveMultipliers[key] = value; break;
            case ModifierType.Multiplier: multipliers[key] = value; break;
            case ModifierType.FinalAdd: postMultiplicativeAdditives[key] = value; break;
            default: throw new InvalidOperationException("Use AddModifier for SignFlip.");
        }
        Invalidate(); // 재계산 요청 (dirty 플래그 설정)
    }

    public override void AddModifier(ModifierType type, ModifierKey key)
    {
        if (type != ModifierType.SignFlip)
            throw new InvalidOperationException("Only SignFlip can be added without a value.");
        signModifiers.Add(key);
        Invalidate(); // 재계산 요청 (dirty 플래그 설정)
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
        if (removed) Invalidate(); // 재계산 요청 (dirty 플래그 설정)
    }
}