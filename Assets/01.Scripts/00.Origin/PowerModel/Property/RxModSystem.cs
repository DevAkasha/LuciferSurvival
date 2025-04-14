using System;
using System.Collections.Generic;

public enum ModifierType
{
    OriginAdd,
    AddMultiplier,
    Multiplier,
    FinalAdd,
    SignFlip
}

public interface IModifiable
{
    void ApplyModifier(ModifierKey key, string fieldName, ModifierType type, object value);
    void ApplySignFlip(ModifierKey key);
    void RemoveModifier(ModifierKey key);
}

public interface IRxReadable<T>
{
    T Value { get; }
    void AddListener(Action<T> listener);
    void RemoveListener(Action<T> listener);
}

public interface IRxModBase
{
    object Value { get; }
    void SetValue(object origin);
    void ResetValue(object origin);
    void SetModifier(ModifierType type, ModifierKey key, object value);
    void AddModifier(ModifierType type, ModifierKey key);
    void RemoveModifier(ModifierType type, ModifierKey key);
    void ClearAll();
}

public interface IRxMod<T> : IRxModBase, IRxReadable<T>
{
    new T Value { get; }
    void SetValue(T origin);
    void ResetValue(T origin);
    void SetModifier(ModifierType type, ModifierKey key, T value);
}

public interface IConditionCheckable
{
    bool Satisfies(Func<object, bool> predicate);
}

public abstract class RxBase : IConditionCheckable
{
    public abstract void ClearRelation();
    public virtual bool Satisfies(Func<object, bool> predicate) => false;
}

public abstract class RxModBase<T> : RxBase, IRxMod<T>, IModifiable
{
    protected T origin;
    protected T cachedValue;
    protected T lastNotifiedValue;
    protected bool dirty = true;

    protected readonly List<Action<T>> listeners = new();
    public string FieldName { get; set; } = string.Empty;

    public T Value
    {
        get
        {
            if (dirty)
                Recalculate();
            return cachedValue;
        }
    }

    object IRxModBase.Value => Value;

    public void AddListener(Action<T> listener)
    {
        if (listener != null)
        {
            listeners.Add(listener);
            listener(Value);
        }
    }

    public void RemoveListener(Action<T> listener) => listeners.Remove(listener);

    public void ForceUpdate() { Invalidate(); _ = Value; }

    public void SetOrigin(T value) { origin = value; Invalidate(); }

    public void SetValue(T value)
    {
        origin = value;
        Invalidate();
        ForceUpdate();
    }

    void IRxModBase.SetValue(object origin)
    {
        if (origin is T val) SetValue(val);
        else throw new InvalidCastException($"Expected {typeof(T).Name}");
    }

    public void ResetValue(T newValue)
    {
        origin = newValue;
        ClearAll();
        cachedValue = newValue;
        lastNotifiedValue = newValue;
    }

    void IRxModBase.ResetValue(object origin)
    {
        if (origin is T val) ResetValue(val);
        else throw new InvalidCastException($"Expected {typeof(T).Name}");
    }

    public override bool Satisfies(Func<object, bool> predicate)
        => predicate?.Invoke(Value) ?? false;

    public override void ClearRelation()
    {
        listeners.Clear();
        ClearAll();
    }

    protected void NotifyAll(T value)
    {
        foreach (var l in listeners)
            l(value);
    }

    protected void Invalidate() => dirty = true;

    protected abstract void Recalculate();
    public abstract void ClearAll();
    public abstract void SetModifier(ModifierType type, ModifierKey key, T value);

    void IRxModBase.SetModifier(ModifierType type, ModifierKey key, object value)
    {
        if (value is T typed) SetModifier(type, key, typed);
        else throw new InvalidCastException($"Expected {typeof(T).Name}");
    }

    public abstract void AddModifier(ModifierType type, ModifierKey key);
    public abstract void RemoveModifier(ModifierType type, ModifierKey key);

    void IRxModBase.AddModifier(ModifierType type, ModifierKey key) => AddModifier(type, key);
    void IRxModBase.RemoveModifier(ModifierType type, ModifierKey key) => RemoveModifier(type, key);
    void IRxModBase.ClearAll() => ClearAll();

    public void ApplyModifier(ModifierKey key, string fieldName, ModifierType type, object value)
    {
        if (!string.Equals(FieldName, fieldName, StringComparison.OrdinalIgnoreCase)) return;
        ((IRxModBase)this).SetModifier(type, key, value);
    }

    public void ApplySignFlip(ModifierKey key) => AddModifier(ModifierType.SignFlip, key);

    public void RemoveModifier(ModifierKey key)
    {
        RemoveModifier(ModifierType.OriginAdd, key);
        RemoveModifier(ModifierType.AddMultiplier, key);
        RemoveModifier(ModifierType.Multiplier, key);
        RemoveModifier(ModifierType.FinalAdd, key);
        RemoveModifier(ModifierType.SignFlip, key);
    }
}

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