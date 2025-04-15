using System;
using System.Collections.Generic;

public sealed class RxStateFlag
{
    private readonly RxVar<bool> internalFlag;
    private Func<bool>? condition;

    public string Name { get; }
    public bool Value => internalFlag.Value;

    public event Action<bool>? OnChanged;

    internal RxStateFlag(string name, object owner = null, Func<bool>? condition = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        this.condition = condition;
        internalFlag = new RxVar<bool>(false, owner);
        internalFlag.AddListener(HandleChange);
    }

    private void HandleChange(bool value)
    {
        OnChanged?.Invoke(value);
    }

    internal void Set(bool value)
    {
        internalFlag.SetValue(value);
    }

    internal void Evaluate()
    {
        if (condition == null) return;
        Set(condition.Invoke());
    }

    internal void SetCondition(Func<bool> newCondition)
    {
        condition = newCondition;
    }

    public void AddListener(Action<bool> listener)
    {
        internalFlag.AddListener(listener);
    }

    public void RemoveListener(Action<bool> listener)
    {
        internalFlag.RemoveListener(listener);
    }

    public override string ToString()
    {
        return $"[RxStateFlag] {Name} = {Value}";
    }
}

public class RxStateFlagSet<TEnum> where TEnum : Enum
{
    private readonly List<RxStateFlag> flags;
    private readonly Dictionary<TEnum, int> indexMap;

    public RxStateFlagSet(object owner = null)
    {
        var values = (TEnum[])Enum.GetValues(typeof(TEnum));
        flags = new List<RxStateFlag>(values.Length);
        indexMap = new Dictionary<TEnum, int>();

        for (int i = 0; i < values.Length; i++)
        {
            var enumValue = values[i];
            indexMap[enumValue] = i;
            flags.Add(new RxStateFlag(enumValue.ToString(), owner));
        }
    }

    public RxStateFlag this[TEnum state] => flags[indexMap[state]];

    public void Set(TEnum state, bool value) => this[state].Set(value);

    public bool GetValue(TEnum state) => this[state].Value;

    public void SetCondition(TEnum state, Func<bool> condition) => this[state].SetCondition(condition);

    public void Evaluate(TEnum state) => this[state].Evaluate();

    public void EvaluateAll()
    {
        foreach (var flag in flags)
            flag.Evaluate();
    }

    public void AddListener(TEnum state, Action<bool> listener) => this[state].AddListener(listener);

    public void RemoveListener(TEnum state, Action<bool> listener) => this[state].RemoveListener(listener);

    public bool AnyActive() => flags.Exists(f => f.Value);
    public bool AllSatisfied() => flags.TrueForAll(f => f.Value);
    public bool NoneActive() => flags.TrueForAll(f => !f.Value);

    public IEnumerable<(TEnum, bool)> Snapshot()
    {
        foreach (var pair in indexMap)
        {
            yield return (pair.Key, flags[pair.Value].Value);
        }
    }

    public IEnumerable<TEnum> ActiveFlags()
    {
        foreach (var (key, value) in Snapshot())
        {
            if (value) yield return key;
        }
    }

    public override string ToString()
    {
        return $"RxStateFlagSet<{typeof(TEnum).Name}>: " + string.Join(", ", Snapshot());
    }
}
