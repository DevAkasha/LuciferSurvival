using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

// 4. 스킬 시스템 통합 관리 클래스
public class SkillManager : Singleton<SkillManager>
{
    [SerializeField] private GameObject projectilePrefab;   // 기본 투사체
    [SerializeField] private GameObject aoeEffectPrefab;    // 범위 효과

    // 스킬 데이터 목록 (인스펙터에서 설정 가능)
    [SerializeField] private List<SkillData> availableSkills = new List<SkillData>();

    // 스킬 데이터 딕셔너리 (빠른 조회용)
    private Dictionary<SkillType, SkillData> skillDataDict = new Dictionary<SkillType, SkillData>();

    protected override void Awake()
    {
        base.Awake();
        // 스킬 데이터 딕셔너리 구성
        foreach (var skill in availableSkills)
        {
            skillDataDict[skill.skillType] = skill;
        }
    }

    // 스킬 데이터 조회
    public SkillData GetSkillData(SkillType skillType)
    {
        if (skillDataDict.TryGetValue(skillType, out var skillData))
            return skillData;

        Debug.LogWarning($"스킬 데이터를 찾을 수 없음: {skillType}");
        return null;
    }

    // 스킬 실행 메서드
    public void ExecuteSkill(SkillType skillType, PlayerEntity caster)
    {
        if (caster == null)
        {
            Debug.LogError("스킬 시전자가 없습니다.");
            return;
        }

        var skillData = GetSkillData(skillType);
        if (skillData == null) return;

        // 애니메이션 트리거 설정
        if (!string.IsNullOrEmpty(skillData.animationTrigger))
        {
            caster.Model.Flags.SetValue(PlayerStateFlag.Attack, true);
        }

        // 효과음 재생
        if (skillData.soundEffect != null)
        {
            AudioSource.PlayClipAtPoint(skillData.soundEffect, caster.transform.position);
        }

        // 스킬 이펙트 생성
        if (skillData.effectPrefab != null)
        {
            Instantiate(skillData.effectPrefab, caster.transform.position, caster.transform.rotation);
        }

        // 스킬 타입별 실행
        switch (skillType)
        {
            case SkillType.MistOfMadness:
            case SkillType.FleshGrinder:
                ExecuteCircleAoE(caster, skillData);
                break;

            case SkillType.CrushBreaker:
                ExecuteForwardAoE(caster, skillData);
                break;

            case SkillType.Hellfume:
                ExecuteGroundAoE(caster, skillData);
                break;

            case SkillType.SkyLance:
                ExecutePenetrationProjectile(caster, skillData);
                break;

            case SkillType.OrbitOfRuin:
                ExecuteMultiProjectile(caster, skillData);
                break;

            case SkillType.InfernalVolley:
                ExecuteRapidProjectile(caster, skillData);
                break;

            case SkillType.Hellsnare:
                ExecuteSingleProjectile(caster, skillData);
                break;
        }
    }

    // 플레이어 주변 원형 범위 공격
    private void ExecuteCircleAoE(PlayerEntity caster, SkillData skillData)
    {
        var targets = Physics.OverlapSphere(caster.transform.position, skillData.radius, 1 << LayerMask.NameToLayer("Enemy"));

        foreach (var target in targets)
        {
            var entity = target.GetComponent<AngelEntity>();
            if (entity == null) continue;

            // 데미지 적용
            entity.TakeDamaged(skillData.damage);

            // 상태 이상 효과 적용
            ApplyStatusEffects(entity, skillData);
        }
    }

    // 전방 부채꼴 범위 공격
    private void ExecuteForwardAoE(PlayerEntity caster, SkillData skillData)
    {
        // 전방 부채꼴 범위의 적 찾기 (단순화를 위해 원 범위로 처리하고 전방 각도만 체크)
        var targets = Physics.OverlapSphere(
            caster.transform.position + caster.transform.forward * skillData.radius * 0.5f,
            skillData.radius,
            1 << LayerMask.NameToLayer("Enemy")
        );

        foreach (var target in targets)
        {
            var entity = target.GetComponent<AngelEntity>();
            if (entity == null) continue;

            // 전방 120도 각도 내에 있는지 확인
            Vector3 directionToTarget = (entity.transform.position - caster.transform.position).normalized;
            float angle = Vector3.Angle(caster.transform.forward, directionToTarget);

            if (angle <= 60f) // 전방 120도 (양쪽 60도)
            {
                // 데미지 적용
                entity.TakeDamaged(skillData.damage);

                // 상태 이상 효과 적용
                ApplyStatusEffects(entity, skillData);
            }
        }
    }

    // 지정 위치 장판 공격
    private void ExecuteGroundAoE(PlayerEntity caster, SkillData skillData)
    {
        // 장판 생성 위치 (플레이어 전방)
        Vector3 groundPos = caster.transform.position + caster.transform.forward * skillData.radius * 1.5f;

        // 장판 효과 생성
        GameObject aoeEffect = Instantiate(aoeEffectPrefab ?? skillData.effectPrefab, groundPos, Quaternion.identity);

        // 크기 조정
        aoeEffect.transform.localScale = new Vector3(skillData.radius, 1f, skillData.radius);

        // 일정 시간 후 제거
        Destroy(aoeEffect, skillData.duration);

        // 장판 데미지 코루틴 실행
        StartCoroutine(GroundAoEDamageCoroutine(groundPos, skillData));
    }

    // 장판 데미지 코루틴
    private IEnumerator GroundAoEDamageCoroutine(Vector3 position, SkillData skillData)
    {
        float elapsed = 0f;
        float tickInterval = 1f; // 1초마다 데미지

        while (elapsed < skillData.duration)
        {
            // 범위 내 적들에게 데미지
            var targets = Physics.OverlapSphere(position, skillData.radius, 1 << LayerMask.NameToLayer("Enemy"));

            foreach (var target in targets)
            {
                var entity = target.GetComponent<AngelEntity>();
                if (entity == null) continue;

                // 데미지 적용
                entity.TakeDamaged(skillData.damage);

                // 상태 이상 효과 적용
                ApplyStatusEffects(entity, skillData);
            }

            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
    }

    // 관통 투사체 발사
    private void ExecutePenetrationProjectile(PlayerEntity caster, SkillData skillData)
    {
        // 기본 투사체 생성
        GameObject projectileObj = Instantiate(
            projectilePrefab,
            caster.transform.position + caster.transform.forward,
            caster.transform.rotation
        );

        // 투사체 컴포넌트 가져오기
        var projectile = projectileObj.GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError("투사체 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // 투사체 초기화
        projectile.Init(5f, 20f, skillData.damage, null);

        // 투사체의 OnTriggerEnter에서 아래 로직 실행 (실제 구현은 Projectile 클래스에서 해야 함)
        // 단, 관통 로직은 히트 시 바로 파괴되지 않고 관통 횟수를 카운트하는 방식으로 구현해야 함
    }

    // 여러 방향 투사체 발사
    private void ExecuteMultiProjectile(PlayerEntity caster, SkillData skillData)
    {
        int shotCount = 5; // 기본값
        float angleStep = 360f / shotCount;

        for (int i = 0; i < shotCount; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = caster.transform.rotation * Quaternion.Euler(0, angle, 0);

            // 투사체 생성
            GameObject projectileObj = Instantiate(
                projectilePrefab,
                caster.transform.position,
                rotation
            );

            // 투사체 초기화
            var projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Init(5f, 15f, skillData.damage, null);
            }
        }
    }

    // 빠른 연속 투사체 발사
    private void ExecuteRapidProjectile(PlayerEntity caster, SkillData skillData)
    {
        StartCoroutine(RapidFireCoroutine(caster, skillData));
    }

    // 연속 발사 코루틴
    private IEnumerator RapidFireCoroutine(PlayerEntity caster, SkillData skillData)
    {
        int shotCount = 10;
        float delayBetweenShots = 0.1f;

        for (int i = 0; i < shotCount; i++)
        {
            // 투사체 생성
            GameObject projectileObj = Instantiate(
                projectilePrefab,
                caster.transform.position + caster.transform.forward,
                caster.transform.rotation
            );

            // 투사체 초기화
            var projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Init(5f, 20f, skillData.damage, null);
            }

            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    // 단일 투사체 발사
    private void ExecuteSingleProjectile(PlayerEntity caster, SkillData skillData)
    {
        // 투사체 생성
        GameObject projectileObj = Instantiate(
            projectilePrefab,
            caster.transform.position + caster.transform.forward,
            caster.transform.rotation
        );

        // 투사체 초기화
        var projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Init(5f, 15f, skillData.damage, null);
        }
    }

    // 상태 이상 효과 적용
    private void ApplyStatusEffects(AngelEntity entity, SkillData skillData)
    {
        if (entity == null) return;

        // 슬로우
        if (skillData.applySlow)
        {
            ModifierEffect slowEffect = EffectManager.Instance.GetModifierEffect(EffectId.Slow);
            new EffectApplier(slowEffect).AddTarget(entity).Apply();
        }

        // 넉백
        if (skillData.applyKnockback)
        {
            entity.OnKnockBack(5f, entity.transform.position - entity.transform.forward * 2f);
        }

        // 스턴
        if (skillData.applyStun)
        {
            entity.OnStun(skillData.duration);
        }

        // 혼란
        if (skillData.applyConfusion)
        {
            entity.OnConfuse(skillData.duration);
        }

        // 에어본
        if (skillData.applyAirborne)
        {
            // 에어본은 위로 띄우고 스턴과 같이 사용
            ModifierEffect airborneEffect = EffectManager.Instance.GetModifierEffect(EffectId.Airborne);
            new EffectApplier(airborneEffect).AddTarget(entity).Apply();
            entity.OnKnockBack(3f, entity.transform.position + Vector3.up * 3);
        }
    }
}

// 5. 플레이어 스킬 모듈
public class SimplePlayerSkillModule : PlayerPart
{
    private SkillManager skillSystem;

    [SerializeField] private List<SkillType> equippedSkills = new List<SkillType>();

    // 스킬과 쿨다운 정보를 담을 리스트
    public readonly List<(float cooldown, Action skill)> PlayerSkillList = new List<(float cooldown, Action skill)>();

    protected override void AtInit()
    {
        skillSystem = SkillManager.Instance;
        RegisterSkills();
    }

    private void RegisterSkills()
    {
        PlayerSkillList.Clear();

        // 장착된 스킬들을 등록
        foreach (var skillType in equippedSkills)
        {
            var skillData = skillSystem.GetSkillData(skillType);
            if (skillData == null) continue;

            // 스킬 실행 액션을 생성하여 등록
            Action skillAction = () => ExecuteSkill(skillType);
            PlayerSkillList.Add((skillData.cooldown, skillAction));
        }
    }

    private void ExecuteSkill(SkillType skillType)
    {
        skillSystem.ExecuteSkill(skillType, Entity);
    }
}

// 6. 확장된 스킬 UI 프리젠터
public class SimpleSkillPresenter : MonoBehaviour
{
    [SerializeField] private CooldownButton[] skillButtons;
    [SerializeField] private SimplePlayerSkillModule playerSkillModule;

    // 스킬 아이콘 이미지 배열
    [SerializeField] private Sprite[] skillIcons;

    private void Start()
    {
        if (playerSkillModule == null)
            playerSkillModule = PlayerManager.Instance.Player.GetComponentInChildren<SimplePlayerSkillModule>();

        if (playerSkillModule == null)
        {
            Debug.LogError("PlayerSkillModule을 찾을 수 없습니다.");
            return;
        }

        Initialize();
    }

    public void Initialize()
    {
        var skillList = playerSkillModule.PlayerSkillList;

        for (int i = 0; i < skillButtons.Length && i < skillList.Count; i++)
        {
            // 쿨다운 버튼 초기화
            skillButtons[i].Initialize(skillList[i]);

            // 아이콘 설정 (있는 경우)
            if (skillIcons != null && skillIcons.Length > i)
            {
                var image = skillButtons[i].GetComponent<UnityEngine.UI.Image>();
                if (image != null)
                {
                    image.sprite = skillIcons[i];
                }
            }
        }
    }
}

public enum SkillType
{
    // 원본 8개 스킬
    FleshGrinder,   // 도살자: 고기 갈기기
    CrushBreaker,   // 파쇄자: 분쇄 강타
    Hellfume,       // 재의손: 지옥분출
    SkyLance,       // 추적자: 천공의 창
    OrbitOfRuin,    // 파편사: 파멸의 궤도
    InfernalVolley, // 속사수: 악마의 연타
    Hellsnare,      // 처형자: 지옥의 족쇄
    MistOfMadness   // 망령술사: 광기의 안개
}

[System.Serializable]
public class SkillData
{
    public SkillType skillType;
    public string skillName;
    public string description;
    public float cooldown = 10f;
    public float damage = 20f;
    public float radius = 5f;
    public float duration = 3f;
    public GameObject effectPrefab;
    public AudioClip soundEffect;

    // 상태 이상 효과
    public bool applySlow;
    public bool applyKnockback;
    public bool applyStun;
    public bool applyConfusion;
    public bool applyAirborne;

    // 애니메이션 파라미터
    public string animationTrigger = "Attack";
}