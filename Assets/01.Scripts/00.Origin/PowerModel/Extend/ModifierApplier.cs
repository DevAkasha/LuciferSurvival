
using System;
using System.Collections.Generic;
using UnityEngine;

public class ModifierApplier
{
    private readonly ModifierEffect effect; // 적용할 modifier 효과 정의
    private readonly List<IModifiableTarget> targets = new();

    public IReadOnlyList<IModifiableTarget> Targets => targets;

    public ModifierApplier(ModifierEffect effect)
    {
        this.effect = effect;
    }

    public ModifierApplier AddTarget(IModifiableTarget target) // modifier를 적용할 대상 추가
    {
        if (target != null)
            targets.Add(target);
        return this;
    }

    public void Apply() // modifier를 대상에게 적용
    {
        foreach (var target in targets)
        {
            if (target == null)
            {
                Debug.LogError("[ModifierApplier] Null IModifiableTarget detected in Apply.");
                continue;
            }

            bool anyApplied = false;

            foreach (var modifiable in target.GetModifiables())
            {
                if (modifiable == null)
                    continue;

                foreach (var modifier in effect.Modifiers)
                {
                    if (modifiable is IRxField field && field.FieldName.Equals(modifier.FieldName, StringComparison.OrdinalIgnoreCase))
                    {
                        modifiable.ApplyModifier(effect.Key, modifier.FieldName, modifier.Type, modifier.Value);
                        anyApplied = true;
                    }
                }

                if (effect.HasSignFlip)
                {
                    try
                    {
                        modifiable.ApplySignFlip(effect.Key);
                        anyApplied = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[ModifierApplier] Failed to apply sign flip: {e.Message}");
                    }
                }
            }

            if (!anyApplied)
                Debug.LogWarning($"[ModifierApplier] No applicable modifiers found for effect '{effect.Key}'.");
        }
    }

    public void Remove() // modifier를 대상에게서 제거
    {
        foreach (var target in targets)
        {
            if (target == null)
            {
                Debug.LogError("[ModifierApplier] Null IModifiableTarget detected in Remove.");
                continue;
            }

            foreach (var modifiable in target.GetModifiables())
            {
                if (modifiable == null)
                    continue;

                try
                {
                    modifiable.RemoveModifier(effect.Key);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ModifierApplier] Failed to remove modifier: {e.Message}");
                }
            }
        }
    }
}
