using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillId
{
    FleshGrinder,   // 고기 갈기기(도살자 스킬)
    CrushBreaker,   // 분쇄 강타(파쇄자 스킬)
    Hellfume,       // 지옥분출(재의손 스킬)
    SkyLance,       // 천공의 창(추적자 스킬)
    OrbitOfRuin,    // 파멸의 궤도(파편사 스킬)
    InfernalVolley, // 악마의 연타(속사수 스킬)
    Hellsnare,      // 지옥의 족쇄(처형자 스킬)
    MistOfMadness   // 광기의 안개(망령술사 스킬)
}

public enum StatusEffectType
{
    None,
    Slow,
    Knockback,
    Stun,
    Confusion,
    Airborne
}

public interface ISkillUser
{
    Transform GetTransform();
    Transform GetPlayerTransform();
    void SetAnimation(string animationTrigger);
}

public interface ISkillTarget
{
    Transform GetTransform();
    void TakeDamaged(float damage);
    void ApplyStatusEffect(StatusEffectType effectType, float duration, float power);
}

public abstract class Skill
{
    // 스킬 속성
    public SkillId id { get; protected set; }
    public string displayName { get; protected set; }
    public string description { get; protected set; }
    public float cooldown { get; protected set; }
    public float damage { get; protected set; }

    // 스킬 실행 메서드
    public abstract void Execute(ISkillUser user);

    // 데미지 적용 메서드
    protected void ApplyDamage(ISkillTarget target, float damage)
    {
        if (target != null)
        {
            target.TakeDamaged(damage);
        }
    }

    // 상태 효과 적용 메서드
    protected void ApplyStatusEffect(ISkillTarget target, StatusEffectType effectType, float duration, float power = 1f)
    {
        if (target != null)
        {
            target.ApplyStatusEffect(effectType, duration, power);
        }
    }
}

public static class SkillFactory
{
    // 스킬 ID로 스킬 객체 생성
    public static Skill CreateSkill(SkillId skillId)
    {
        return skillId switch
        {
            SkillId.FleshGrinder => new FleshGrinderSkill(),
            SkillId.CrushBreaker => new CrushBreakerSkill(),
            SkillId.Hellfume => new HellfumeSkill(),
            SkillId.SkyLance => new SkyLanceSkill(),
            SkillId.OrbitOfRuin => new OrbitOfRuinSkill(),
            SkillId.InfernalVolley => new InfernalVolleySkill(),
            SkillId.Hellsnare => new HellsnareSkill(),
            SkillId.MistOfMadness => new MistOfMadnessSkill(),
            _ => throw new System.ArgumentException($"Unknown skill ID: {skillId}")
        };
    }
}






