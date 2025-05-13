using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSkillComponent : MonoBehaviour
{
    [SerializeField] private Transform unitSlots; // 유닛이 배치된 Transform

    // 유닛 스킬 컴포넌트 캐시
    private List<UnitSkillComponent> unitSkills = new List<UnitSkillComponent>();

    // 쿨다운 UI 연동용 리스트
    public List<(float maxCooldown, float remainingCooldown)> SkillCooldownInfo
    {
        get
        {
            var cooldownList = new List<(float maxCooldown, float remainingCooldown)>();
            foreach (var skill in unitSkills)
            {
                cooldownList.Add(skill.SkillCooldownInfo);
            }
            return cooldownList;
        }
    }

    private void Start()
    {
        // 모든 유닛의 스킬 컴포넌트 찾기
        FindUnitSkills();
    }

    // 모든 유닛의 스킬 컴포넌트 찾기
    public void FindUnitSkills()
    {
        unitSkills.Clear();

        // 유닛 슬롯에서 모든 유닛 찾기
        if (unitSlots != null)
        {
            foreach (Transform unitTransform in unitSlots)
            {
                UnitSkillComponent skillComponent = unitTransform.GetComponentInChildren<UnitSkillComponent>();
                if (skillComponent != null)
                {
                    unitSkills.Add(skillComponent);
                }
            }
        }
    }

    // 인덱스로 스킬 수동 사용 (옵션 기능)
    public void UseSkillManually(int index)
    {
        if (index >= 0 && index < unitSkills.Count)
        {
            unitSkills[index].ManualUseSkill();
        }
    }
}