using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    private readonly Dictionary<ModifierKey, BaseEffect> effects = new();

    public void Register(BaseEffect effect)
    {
        if (effect == null)
        {
            Debug.LogError("[EffectManager] Cannot register null effect");
            return;
        }

        effects[effect.Key] = effect;
    }

    public bool TryGetEffect(ModifierKey key, out BaseEffect effect)
    {
        return effects.TryGetValue(key, out effect);
    }


    public BaseEffect GetEffect(ModifierKey key)
    {
        if (!effects.TryGetValue(key, out var effect))
            throw new KeyNotFoundException($"Effect not found: {key}");
        return effect;
    }

    public T GetEffect<T>(ModifierKey key) where T : BaseEffect
    {
        BaseEffect effect = GetEffect(key);

        if (effect is T typedEffect)
            return typedEffect;
        else
            throw new InvalidCastException($"Effect {key} is not of type {typeof(T).Name}");
    }

    public ModifierEffect GetModifierEffect(ModifierKey key)
    {
        return GetEffect<ModifierEffect>(key);
    }
    public DirectEffect GetDirectEffect(ModifierKey key)
    {
        return GetEffect<DirectEffect>(key);
    }
    protected override void Awake()
    {
        base.Awake();

        Register(EffectBuilder.DefineModifier(EffectId.Exhaust, EffectApplyMode.Timed)
                .Add("MoveSpeed", ModifierType.Multiplier, 0.7f)
                .Duration(2f)
                .Build());

        Register(EffectBuilder.DefineModifier(EffectId.Ghost, EffectApplyMode.Timed)
                .Add("MoveSpeed", ModifierType.Multiplier, 1.8f)
                .Duration(10f)
                .Build());

        Register(EffectBuilder.DefineModifier(EffectId.Slow, EffectApplyMode.Timed)
                .Add("MoveSpeed", ModifierType.Multiplier, 0.5f)
                .Duration(3f)
                .Build());

        Register(EffectBuilder.DefineModifier(EffectId.Airborne, EffectApplyMode.Timed)
                .Add("MoveSpeed", ModifierType.Multiplier, 0f)
                .Duration(2f)
                .Build());
    }
}

public enum EffectId
{
    // 플레이어 스킬 효과
    Ghost,
    Exhaust,

    // 상태 이상 효과
    Slow,
    Knockback,
    Stun,
    Confusion,
    Airborne,

    // 스킬 효과
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
public class SkillInfo
{
    public EffectId effectId;
    public string displayName;
    public string description;
    public float cooldown = 10f;
    public Sprite icon;
}

public partial class EffectManager
{
    [Header("스킬 프리팹")]
    [SerializeField] private EnhancedProjectile projectilePrefab;
    [SerializeField] private EnhancedProjectile multiShotPrefab;
    [SerializeField] private EnhancedProjectile rapidShotPrefab;
    [SerializeField] private EnhancedProjectile aoePrefab;
    [SerializeField] private EnhancedProjectile groundEffectPrefab;

    // 스킬 실행 메서드들

    // 1. 도살자: 고기 갈기기 (원형 범위 공격 + 슬로우)
    public void ExecuteFleshGrinder(PlayerEntity caster, float damage = 25f, float radius = 5f)
    {
        // 플레이어 주변 원형 범위 공격
        var targets = Physics.OverlapSphere(caster.transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));

        foreach (var target in targets)
        {
            var entity = target.GetComponent<AngelEntity>();
            if (entity == null) continue;

            // 데미지 적용
            entity.TakeDamaged(damage);

            // 슬로우 효과 적용
            var slowEffect = GetModifierEffect(EffectId.Slow);
            new EffectApplier(slowEffect).AddTarget(entity).Apply();
        }

        // 이펙트 생성
        var effect = Instantiate(aoePrefab, caster.transform.position, Quaternion.identity);
        effect.aoeRadius = radius;
        effect.damage = damage;

        // 슬로우 효과 설정
        var statusEffect = effect.GetComponent<ProjectileStatusEffect>();
        if (statusEffect != null)
        {
            statusEffect.slowAmount = 0.5f;
            statusEffect.slowDuration = 3f;
        }

        // 애니메이션 설정
        caster.Model.Flags.SetValue(PlayerStateFlag.Attack, true);
    }

    // 2. 파쇄자: 분쇄 강타 (전방 공격 + 넉백)
    public void ExecuteCrushBreaker(PlayerEntity caster, float damage = 30f, float radius = 4f)
    {
        // 전방 위치 계산
        Vector3 forwardPos = caster.transform.position + caster.transform.forward * 2f;

        // 전방 부채꼴 범위의 적 찾기 (반원 형태로 단순화)
        var targets = Physics.OverlapSphere(forwardPos, radius, 1 << LayerMask.NameToLayer("Enemy"));

        foreach (var target in targets)
        {
            var entity = target.GetComponent<AngelEntity>();
            if (entity == null) continue;

            // 전방 180도 내에 있는지 확인
            Vector3 dirToTarget = (entity.transform.position - caster.transform.position).normalized;
            float dotProduct = Vector3.Dot(caster.transform.forward, dirToTarget);

            if (dotProduct > 0) // 전방 180도 내
            {
                // 데미지 적용
                entity.TakeDamaged(damage);

                // 넉백 효과 적용
                entity.OnKnockBack(5f, forwardPos);
            }
        }

        // 이펙트 생성
        var effect = Instantiate(aoePrefab, forwardPos, caster.transform.rotation);
        effect.aoeRadius = radius;
        effect.damage = damage;

        // 넉백 효과 설정
        var statusEffect = effect.GetComponent<ProjectileStatusEffect>();
        if (statusEffect != null)
        {
            statusEffect.knockbackPower = 5f;
        }

        // 애니메이션 설정
        caster.Model.Flags.SetValue(PlayerStateFlag.Attack, true);
    }

    // 3. 재의손: 지옥분출 (장판 공격)
    public void ExecuteHellfume(PlayerEntity caster, float damage = 20f, float radius = 4f, float duration = 5f)
    {
        // 장판 생성 위치 (플레이어 전방)
        Vector3 groundPos = caster.transform.position + caster.transform.forward * radius * 1.5f;

        // 장판 이펙트 생성
        var effect = Instantiate(groundEffectPrefab, groundPos, Quaternion.identity);
        effect.aoeRadius = radius;
        effect.damage = damage;

        // 장판 데미지 코루틴 시작
        StartCoroutine(GroundEffectDamageCoroutine(groundPos, radius, damage, duration));

        // 애니메이션 설정
        caster.Model.Flags.SetValue(PlayerStateFlag.Cast, true);
    }

    private IEnumerator GroundEffectDamageCoroutine(Vector3 position, float radius, float damage, float duration)
    {
        float elapsed = 0f;
        float tickInterval = 1f; // 1초마다 데미지

        while (elapsed < duration)
        {
            // 범위 내 적들에게 데미지
            var targets = Physics.OverlapSphere(position, radius, 1 << LayerMask.NameToLayer("Enemy"));

            foreach (var target in targets)
            {
                var entity = target.GetComponent<AngelEntity>();
                if (entity == null) continue;

                // 데미지 적용
                entity.TakeDamaged(damage);
            }

            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
    }

    // 4. 추적자: 천공의 창 (관통 투사체 + 에어본)
    public void ExecuteSkyLance(PlayerEntity caster, float damage = 35f, int penetrationCount = 3)
    {
        // 투사체 생성
        var projectile = Instantiate(projectilePrefab,
                                    caster.transform.position + caster.transform.forward,
                                    caster.transform.rotation);

        // 투사체 설정
        projectile.isPenetrating = true;
        projectile.penetrationCount = penetrationCount;
        projectile.damage = damage;
        projectile.Init(5f, 20f, damage, null);

        // 에어본 효과 설정
        var statusEffect = projectile.GetComponent<ProjectileStatusEffect>();
        if (statusEffect != null)
        {
            statusEffect.airborneDuration = 2f;
        }

        // 애니메이션 설정
        caster.Model.Flags.SetValue(PlayerStateFlag.Attack, true);
    }

    // 5. 파편사: 파멸의 궤도 (멀티샷)
    public void ExecuteOrbitOfRuin(PlayerEntity caster, float damage = 15f, int shotCount = 5)
    {
        // 여러 방향으로 투사체 발사
        float angleStep = 360f / shotCount;

        for (int i = 0; i < shotCount; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = caster.transform.rotation * Quaternion.Euler(0, angle, 0);

            // 투사체 생성
            var projectile = Instantiate(multiShotPrefab,
                                        caster.transform.position,
                                        rotation);

            // 투사체 설정
            projectile.damage = damage;
            projectile.Init(5f, 15f, damage, null);
        }

        // 애니메이션 설정
        caster.Model.Flags.SetValue(PlayerStateFlag.Attack, true);
    }

    // 6. 속사수: 악마의 연타 (속사)
    public void ExecuteInfernalVolley(PlayerEntity caster, float damage = 10f, int shotCount = 10)
    {
        // 연속 발사 코루틴 시작
        StartCoroutine(RapidFireCoroutine(caster, damage, shotCount));
    }

    private IEnumerator RapidFireCoroutine(PlayerEntity caster, float damage, int shotCount)
    {
        float delayBetweenShots = 0.1f;

        for (int i = 0; i < shotCount; i++)
        {
            // 투사체 생성
            var projectile = Instantiate(rapidShotPrefab,
                                        caster.transform.position + caster.transform.forward,
                                        caster.transform.rotation);

            // 투사체 설정
            projectile.damage = damage;
            projectile.Init(5f, 20f, damage, null);

            yield return new WaitForSeconds(delayBetweenShots);
        }

        // 애니메이션 설정
        caster.Model.Flags.SetValue(PlayerStateFlag.Attack, true);
    }

    // 7. 처형자: 지옥의 족쇄 (단일 투사체 + 스턴)
    public void ExecuteHellsnare(PlayerEntity caster, float damage = 25f)
    {
        // 투사체 생성
        var projectile = Instantiate(projectilePrefab,
                                    caster.transform.position + caster.transform.forward,
                                    caster.transform.rotation);

        // 투사체 설정
        projectile.damage = damage;
        projectile.Init(5f, 15f, damage, null);

        // 스턴 효과 설정
        var statusEffect = projectile.GetComponent<ProjectileStatusEffect>();
        if (statusEffect != null)
        {
            statusEffect.stunDuration = 3f;
        }

        // 애니메이션 설정
        caster.Model.Flags.SetValue(PlayerStateFlag.Attack, true);
    }

    // 8. 망령술사: 광기의 안개 (광역 혼란)
    public void ExecuteMistOfMadness(PlayerEntity caster, float damage = 15f, float radius = 8f)
    {
        // 플레이어 주변 광역 공격
        var targets = Physics.OverlapSphere(caster.transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));

        foreach (var target in targets)
        {
            var entity = target.GetComponent<AngelEntity>();
            if (entity == null) continue;

            // 데미지 적용
            entity.TakeDamaged(damage);

            // 혼란 효과 적용
            entity.OnConfuse(5f);
        }

        // 이펙트 생성
        var effect = Instantiate(aoePrefab, caster.transform.position, Quaternion.identity);
        effect.aoeRadius = radius;
        effect.damage = damage;

        // 혼란 효과 설정
        var statusEffect = effect.GetComponent<ProjectileStatusEffect>();
        if (statusEffect != null)
        {
            statusEffect.confuseDuration = 5f;
        }

        // 애니메이션 설정
        caster.Model.Flags.SetValue(PlayerStateFlag.Cast, true);
    }
}