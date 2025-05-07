using System;
using System.Collections.Generic;
using UnityEngine;

public enum EffectApplyMode { Passive, Manual, Timed } // modifier 적용 방식 (수동, 자동, 시간제한)

public readonly struct FieldModifier // 필드에 적용할 modifier 정보를 담는 구조체
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

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Game/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    public List<SkillInfo> skills = new List<SkillInfo>();

    // 효과 ID로 스킬 정보 검색
    public SkillInfo GetSkillInfo(EffectId effectId)
    {
        return skills.Find(skill => skill.effectId == effectId);
    }
}

public static class SkillEffectBuilder
{
    public static EffectBuilder.ModifierEffectBuilder Define(Enum effectId, EffectApplyMode mode = EffectApplyMode.Manual, float duration = 0f)
        => EffectBuilder.DefineModifier(effectId, mode, duration);
}