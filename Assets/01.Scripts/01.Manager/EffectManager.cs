using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager: Singleton<EffectManager>
{
    private readonly Dictionary<ModifierKey, ModifierEffect> effects = new();

    public void Register(ModifierEffect effect)
    {
        effects[effect.Key] = effect;
    }

    public bool TryGetEffect(ModifierKey key, out ModifierEffect effect)
    {
        return effects.TryGetValue(key, out effect);
    }

    public ModifierEffect GetEffect(ModifierKey key)
    {
        if (!effects.TryGetValue(key, out var effect))
            throw new KeyNotFoundException($"Effect not found: {key}");
        return effect;
    }
}
public class EffectRunner : MonoBehaviour
{
    public void ApplyTimedEffect(ModifierEffect effect, ModifierApplier applier)
    {
        applier.Apply();

        if (effect.Mode == EffectApplyMode.Timed)
            StartCoroutine(RemoveAfter(effect.Duration, applier));
    }

    private IEnumerator RemoveAfter(float seconds, ModifierApplier applier)
    {
        yield return new WaitForSeconds(seconds);
        applier.Remove();
    }
}