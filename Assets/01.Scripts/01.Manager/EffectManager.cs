using System.Collections.Generic;

public class EffectManager : Singleton<EffectManager>
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

        Register(SkillEffectBuilder.Define(EffectId.Exhaust, EffectApplyMode.Timed)
            .Add("MoveSpeed", ModifierType.Multiplier, 0.7f)
            .Duration(2f)
            .ToEffect());

        Register(SkillEffectBuilder.Define(EffectId.Ghost, EffectApplyMode.Timed)
            .Add("MoveSpeed", ModifierType.Multiplier, 1.8f)
            .Duration(10f)
            .ToEffect());
    }
}

public enum EffectId
{
    Ghost,
    Exhaust
}