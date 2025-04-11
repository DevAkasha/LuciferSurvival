
using System;
using System.Collections.Generic;
using UnityEngine;

public enum EffectApplyMode { Passive, Manual, Timed }

public interface IModifiable
{
    void ApplySignFlip(ModifierKey key);
    void RemoveModifier(ModifierKey key);
    void ApplyModifier(ModifierKey key, ModifierType type, object value);
}

public interface IModifiableTarget
{
    IEnumerable<IModifiable> GetModifiables();
}

public class ModifierEffect
{
    public ModifierKey Key { get; }
    public EffectApplyMode Mode { get; }
    public float Duration { get; }

    public IReadOnlyList<(ModifierType type, object value)> Modifiers => modifiers;
    public bool HasSignFlip => hasSignFlip;

    private readonly List<(ModifierType type, object value)> modifiers = new();
    private bool hasSignFlip = false;

    public ModifierEffect(Enum id, EffectApplyMode mode = EffectApplyMode.Manual, float duration = 0f)
    {
        Key = id;
        Mode = mode;
        Duration = duration;
    }

    public ModifierEffect Add<T>(ModifierType type, T value)
    {
        if (value is not int and not long and not float and not double)
            throw new InvalidOperationException($"Unsupported modifier type: {typeof(T)}");

        modifiers.Add((type, value!));
        return this;
    }

    public ModifierEffect AddSignFlip()
    {
        hasSignFlip = true;
        return this;
    }
}

public class ModifierApplier
{
    private readonly ModifierEffect effect;
    private readonly List<IModifiableTarget> targets = new();

    public ModifierApplier(ModifierEffect effect)
    {
        this.effect = effect;
    }

    public ModifierApplier AddTarget(IModifiableTarget model)
    {
        targets.Add(model);
        return this;
    }

    public void Apply()
    {
        foreach (var model in targets)
        {
            foreach (var target in model.GetModifiables())
            {
                foreach (var (type, value) in effect.Modifiers)
                {
                    try
                    {
                        target.ApplyModifier(effect.Key, type, value);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"[ModifierApplier] Failed to apply modifier {type} with value {value} to {target.GetType().Name}: {e.Message}");
                    }
                }

                if (effect.HasSignFlip)
                {
                    try
                    {
                        target.ApplySignFlip(effect.Key);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"[ModifierApplier] Failed to apply sign flip to {target.GetType().Name}: {e.Message}");
                    }
                }
            }
        }
    }

    public void Remove()
    {
        if (targets.Count == 0)
        {
            Debug.LogWarning("[ModifierApplier] No targets to remove effect from.");
            return;
        }

        foreach (var model in targets)
        {
            if (model == null)
            {
                Debug.LogError("[ModifierApplier] Found null model during Remove.");
                continue;
            }

            foreach (var target in model.GetModifiables())
            {
                if (target == null)
                {
                    Debug.LogError("[ModifierApplier] Found null IModifiable during Remove.");
                    continue;
                }

                try
                {
                    target.RemoveModifier(effect.Key);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ModifierApplier] Failed to remove modifier: {e.Message}");
                }
            }
        }
    }
}
