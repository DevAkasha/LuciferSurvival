using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class UnitSkillComponent : UnitPart, ISkillUser
{
    // 플레이어 참조
    [SerializeField] private Transform playerTransform;

    // 스킬 설정
    [SerializeField] private SkillId skillId;
    private Skill skill;
    private float cooldownRemaining = 0f;

    // 자동 스킬 사용 설정
    [SerializeField] private bool useAutoSkill = true;
    [SerializeField] private float detectionRange = 5f; // 적 감지 범위
    private LayerMask enemyLayer;

    // 스킬 UI 데이터
    public (float cooldown, float remainingCooldown) SkillCooldownInfo
    {
        get => (skill?.cooldown ?? 0f, cooldownRemaining);
    }

    protected override void AtInit()
    {
        if (playerTransform == null)
            playerTransform = PlayerManager.Instance.Player.transform;

        // 적 레이어 설정
        enemyLayer = LayerMask.GetMask("Enemy");

        // 스킬 초기화
        InitializeSkill();
    }

    private void Update()
    {
        // 쿨다운 업데이트
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining < 0)
                cooldownRemaining = 0;
        }

        // 자동 스킬 사용 로직
        if (useAutoSkill && cooldownRemaining <= 0)
        {
            TryAutoUseSkill();
        }
    }

    // 스킬 초기화
    private void InitializeSkill()
    {
        // 스킬 생성
        skill = SkillFactory.CreateSkill(skillId);
    }

    // 자동 스킬 사용 시도
    private void TryAutoUseSkill()
    {
        // 스킬이 없으면 리턴
        if (skill == null)
            return;

        // 주변 적 감지
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

        // 적이 있으면 스킬 사용
        if (enemies.Length > 0)
        {
            UseSkill();
        }
    }

    // 스킬 사용
    public void UseSkill()
    {
        if (skill == null)
        {
            Debug.LogWarning($"Skill not initialized for unit: {gameObject.name}");
            return;
        }

        // 쿨다운 체크
        if (cooldownRemaining > 0)
        {
            Debug.Log($"Skill {skillId} is on cooldown: {cooldownRemaining:F1}s remaining");
            return;
        }

        Transform target = Entity.curTarget;

        if (skill is ITargetedSkill targetedSkill && target != null)
        {
            targetedSkill.ExecuteWithTarget(this, Entity.curTarget, Entity.curTargetType);
        }
        else
        {
            skill.Execute(this);
        }

        // 쿨다운 설정
        cooldownRemaining = skill.cooldown;
    }

    // 수동으로 스킬 사용 (기존 기능 유지)
    public void ManualUseSkill()
    {
        UseSkill();
    }

    // ISkillUser 인터페이스 구현
    public Transform GetTransform()
    {
        return transform;
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public void SetAnimation(string animationTrigger)
    {
        // 유닛 애니메이션 설정
        // 여기에서는 임시로 로그만 출력
        Debug.Log($"Unit {gameObject.name} animation: {animationTrigger}");

        // 실제 구현은 유닛의 애니메이션 시스템에 맞게 설정
        // 예를 들어 Animator 컴포넌트를 사용한다면:
        // Animator animator = GetComponent<Animator>();
        // if (animator != null)
        //     animator.SetTrigger(animationTrigger);
    }
}