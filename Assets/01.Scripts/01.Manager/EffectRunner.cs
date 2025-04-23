using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRunner : Singleton<EffectRunner>
{
    private readonly Dictionary<ModifierKey, Coroutine> activeEffects = new();

    public void ApplyTimedEffect(ModifierEffect effect, ModifierApplier applier)
    {
        if (!effect.Condition())
        {
            Debug.LogWarning($"[EffectRunner] Condition not met for effect: {effect.Key}");
            return;
        }

        if (TryStartEffect(effect, applier, out var coroutine))
        {
            Debug.Log($"[EffectRunner] <color=cyan>Effect START</color> - {effect.Key}, Duration: {effect.Duration}s");
            if (!effect.Stackable)
                activeEffects[effect.Key] = coroutine;
        }
    }

    private bool TryStartEffect(ModifierEffect effect, ModifierApplier applier, out Coroutine coroutine)
    {
        if (activeEffects.TryGetValue(effect.Key, out var existing))
        {
            if (!effect.Stackable)
            {
                if (effect.RefreshOnDuplicate)
                {
                    Debug.Log($"[EffectRunner] Refreshing effect: {effect.Key}");
                    StopCoroutine(existing);
                    coroutine = StartCoroutine(RunEffect(effect, applier));
                    return true;
                }
                else
                {
                    Debug.Log($"[EffectRunner] Skipping duplicate effect: {effect.Key}");
                    coroutine = null;
                    return false;
                }
            }
        }

        coroutine = StartCoroutine(RunEffect(effect, applier));
        return true;
    }

    private IEnumerator RunEffect(ModifierEffect effect, ModifierApplier applier)
    {
        applier.Apply();

        if (effect.RemoveTrigger != null)
        {
            yield return CheckRemoveTrigger(effect.RemoveTrigger);
        }
        else if (effect.Mode == EffectApplyMode.Timed)
        {
            yield return new WaitForSeconds(effect.Duration);
        }

        applier.Remove();
        Debug.Log($"[EffectRunner] <color=yellow>Effect END</color> - {effect.Key}");

        if (!effect.Stackable)
        {
            activeEffects.Remove(effect.Key);
        }
    }

    private IEnumerator CheckRemoveTrigger(Func<bool> trigger)
    {
        while (!trigger())
        {
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ApplyInterpolatedEffect(ModifierEffect effect, ModifierApplier applier)
    {
        foreach (var target in applier.Targets)
        {
            Instance.StartCoroutine(RunInterpolatedModifier(effect, target));
        }
    }

    private IEnumerator RunInterpolatedModifier(ModifierEffect effect, IModifiableTarget target)
    {
        float duration = effect.Duration;
        float time = 0f;
        ModifierKey key = effect.Key;

        // ModifierEffect에 등록된 필드 목록
        var modifiers = effect.Modifiers;

        while (time < duration)
        {
            float t = time / duration;
            object value = effect.Interpolator.Invoke(t);

            foreach (var modifiable in target.GetModifiables())
            {
                if (modifiable is not IRxField rxField)
                    continue;

                foreach (var modifier in modifiers)
                {
                    // 필드명이 일치할 경우에만 적용
                    if (!string.Equals(rxField.FieldName, modifier.FieldName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (modifiable is IRxModBase mod)
                    {
                        try
                        {
                            mod.SetModifier(modifier.Type, key, value);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[InterpolatedEffect] Failed to set modifier: {e.Message}");
                        }
                    }
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        // 종료 시 모든 modifier 제거
        foreach (var modifiable in target.GetModifiables())
        {
            modifiable.RemoveModifier(key);
        }

        Debug.Log($"[InterpolatedEffect] Modifier removed: {key}");
    }
}
