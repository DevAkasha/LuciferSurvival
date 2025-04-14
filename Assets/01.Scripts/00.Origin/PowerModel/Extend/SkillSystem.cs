using System;
using System.Collections.Generic;

public enum EffectApplyMode { Passive, Manual, Timed }

public readonly struct FieldModifier
{
    public readonly string FieldName;
    public readonly ModifierType Type;
    public readonly object Value;

    public FieldModifier(string fieldName, ModifierType type, object value)
    {
        FieldName = fieldName;
        Type = type;
        Value = value;
    }
}

public class ModifierEffect
{
    public ModifierKey Key { get; }
    public EffectApplyMode Mode { get; private set; }
    public float Duration { get; private set; }
    public IReadOnlyList<FieldModifier> Modifiers => modifiers;
    public bool HasSignFlip => hasSignFlip;
    public bool Stackable { get; private set; } = false;
    public bool RefreshOnDuplicate { get; private set; } = false;
    public Func<bool> RemoveTrigger { get; private set; } = null;

    private readonly List<FieldModifier> modifiers = new();
    private bool hasSignFlip = false;

    private readonly List<Func<bool>> conditions = new();

    public Func<bool> Condition => () =>
    {
        foreach (var cond in conditions)
        {
            if (!cond()) return false;
        }
        return true;
    };

    public ModifierEffect(Enum id, EffectApplyMode mode = EffectApplyMode.Manual, float duration = 0f)
    {
        Key = id;
        Mode = mode;
        Duration = duration;
    }

    public ModifierEffect Add<T>(string fieldName, ModifierType type, T value)
    {
        if (value is not int and not long and not float and not double)
            throw new InvalidOperationException($"Unsupported modifier type: {typeof(T)}");

        modifiers.Add(new FieldModifier(fieldName, type, value!));
        return this;
    }

    public ModifierEffect AddSignFlip()
    {
        hasSignFlip = true;
        return this;
    }

    public ModifierEffect When(Func<bool> condition)
    {
        if (condition != null)
            conditions.Add(condition);
        return this;
    }

    public ModifierEffect Until(Func<bool> trigger)
    {
        RemoveTrigger = trigger;
        return this;
    }

    public ModifierEffect SetDuration(float seconds)
    {
        if (Mode != EffectApplyMode.Timed)
            throw new InvalidOperationException("SetDuration is only valid for Timed effects.");
        Duration = seconds;
        return this;
    }

    public ModifierEffect AllowStacking(bool value = true)
    {
        Stackable = value;
        return this;
    }

    public ModifierEffect RefreshOnRepeat(bool value = true)
    {
        RefreshOnDuplicate = value;
        return this;
    }
}

public static class SkillEffectBuilder
{
    public static SkillEffectDefinition Define(Enum effectId)
        => new(effectId);
}

public class SkillEffectDefinition
{
    private readonly ModifierEffect effect;

    public SkillEffectDefinition(Enum effectId)
    {
        effect = new ModifierEffect(effectId);
    }

    public SkillEffectDefinition Duration(float seconds)
    {
        effect.SetDuration(seconds);
        return this;
    }

    public SkillEffectDefinition Add<T>(string fieldName, ModifierType type, T value)
    {
        effect.Add(fieldName, type, value);
        return this;
    }

    public SkillEffectDefinition AddSignFlip()
    {
        effect.AddSignFlip();
        return this;
    }

    public SkillEffectDefinition When(Func<bool> condition)
    {
        effect.When(condition);
        return this;
    }

    public SkillEffectDefinition Until(Func<bool> trigger)
    {
        effect.Until(trigger);
        return this;
    }

    public SkillEffectDefinition Stackable(bool value = true)
    {
        effect.AllowStacking(value);
        return this;
    }

    public SkillEffectDefinition RefreshOnDuplicate(bool value = true)
    {
        effect.RefreshOnRepeat(value);
        return this;
    }

    public ModifierEffect ToEffect() => effect;
}
