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

    protected override void Awake()
    {
        base.Awake();
        //스킬 등록
        var ghostEffect = new ModifierEffect(EffectId.Ghost, EffectApplyMode.Timed, 10f)
            .Add(ModifierType.Multiplier, 1.8f); // 80% 증가 → ×1.8

        Register(ghostEffect);
    }


}
public enum EffectId
{
    Ghost
}
