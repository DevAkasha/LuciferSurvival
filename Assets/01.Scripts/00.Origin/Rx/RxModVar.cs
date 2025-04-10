using System;
using System.Collections.Generic;

public readonly struct ModifierKey : IEquatable<ModifierKey>
{
    public readonly Enum Id;

    public ModifierKey(Enum id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public override string ToString() => $"{Id.GetType().Name}:{Id}";

    public bool Equals(ModifierKey other) => Equals(Id, other.Id);

    public override bool Equals(object obj) => obj is ModifierKey other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public static implicit operator ModifierKey(Enum value) => new(value);
}

public enum ModifierType
{
    OriginAdd,
    AddMultiplier,
    Multiplier,
    FinalAdd,
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