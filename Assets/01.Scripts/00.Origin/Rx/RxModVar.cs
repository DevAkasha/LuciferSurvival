using System;
using System.Collections.Generic;

public readonly struct ModifierKey : IEquatable<ModifierKey>
{
    public readonly string Type;
    public readonly string Id;

    public ModifierKey(string type, string id)
    {
        Type = type;
        Id = id;
    }

    public bool Equals(ModifierKey other) => Type == other.Type && Id == other.Id;
    public override bool Equals(object obj) => obj is ModifierKey other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Type, Id);
    public override string ToString() => $"{Type}:{Id}";
}

public enum ModifierType
{
    Additive,
    AdditiveMultiplier,
    Multiplier,
    PostMultiplicativeAdditive,
    SignFlip
}
public interface IRxMod<T>
{
    T Value { get; }
    void SetValue(T origin);
    void ResetValue(T origin);
    void SetModifier(ModifierType type, ModifierKey key, T value);
    void AddModifier(ModifierType type, ModifierKey key);
    void RemoveModifier(ModifierType type, ModifierKey key);
    void ClearAll();
}

public interface IRxMod : IUntypedRxMod { } // 기존 구조 호환을 위한 마커
public interface IUntypedRxMod
{
    object Value { get; }
    void SetValue(object origin);
    void ResetValue(object origin);
    void SetModifier(ModifierType type, ModifierKey key, object value);
    void AddModifier(ModifierType type, ModifierKey key);
    void RemoveModifier(ModifierType type, ModifierKey key);
    void ClearAll();
}