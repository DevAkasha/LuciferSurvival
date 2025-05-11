using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleshGrinderSkill : Skill
{
    private float radius = 5f;
    private float slowDuration = 3f;

    public FleshGrinderSkill()
    {
        id = SkillId.FleshGrinder;
        displayName = "고기 갈기기";
        description = "주변 360도를 공격하는 범위 밀집 공격";
        cooldown = 3f;
        damage = 20000f;
    }

    public override void Execute(ISkillUser user)
    {
        if (user == null) return;

        // 애니메이션 설정
        user.SetAnimation("Attack");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetPlayerTransform();
        if (userTransform != null)
        {
            // 원형 이펙트 생성
            GameObject effectObj = GameObject.Instantiate(
                Resources.Load<GameObject>("Effects/FleshGrinderEffect"),
                userTransform.position,
                Quaternion.identity);

            // 이펙트 크기 설정
            effectObj.transform.localScale = new Vector3(radius, 1f, radius);

            // 일정 시간 후 제거
            GameObject.Destroy(effectObj, 2f);

            // 범위 내 적 찾기
            Collider[] colliders = Physics.OverlapSphere(userTransform.position, radius, LayerMask.GetMask("Enemy"));

            // 각 적에게 데미지와 상태 효과 적용
            foreach (var collider in colliders)
            {
                ISkillTarget target = collider.GetComponent<ISkillTarget>();
                if (target != null)
                {
                    ApplyDamage(target, damage);
                    ApplyStatusEffect(target, StatusEffectType.Slow, slowDuration);
                }
            }
        }
    }
}

// 파쇄자: 분쇄 강타 (전방 범위 공격 + 넉백)
public class CrushBreakerSkill : Skill
{
    private float radius = 4f;
    private float knockbackPower = 30f;
    private float angleLimit = 0.5f; // cos(60도) ≈ 0.5, 즉 전방 120도 범위

    public CrushBreakerSkill()
    {
        id = SkillId.CrushBreaker;
        displayName = "분쇄 강타";
        description = "무기를 휘두르며 전방 120도 강타";
        cooldown = 3f;
        damage = 30000f;
    }

    public override void Execute(ISkillUser user)
    {
        if (user == null) return;

        // 애니메이션 설정
        user.SetAnimation("Attack");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetPlayerTransform();
        if (userTransform != null)
        {
            // 전방 위치 계산
            Vector3 forwardPos = userTransform.position + userTransform.forward * 2f;

            // 전방 이펙트 생성
            GameObject effectObj = GameObject.Instantiate(
                Resources.Load<GameObject>("Effects/CrushBreakerEffect"),
                forwardPos,
                userTransform.rotation);

            // 이펙트 크기 설정
            effectObj.transform.localScale = new Vector3(radius, 1f, radius);

            // 일정 시간 후 제거
            GameObject.Destroy(effectObj, 2f);

            // 범위 내 적 찾기
            Collider[] colliders = Physics.OverlapSphere(forwardPos, radius, LayerMask.GetMask("Enemy"));

            // 각 적에게 데미지와 상태 효과 적용 (전방 120도에 있는 경우만)
            foreach (var collider in colliders)
            {
                // 전방 120도 내에 있는지 확인
                Vector3 dirToTarget = (collider.transform.position - userTransform.position).normalized;
                float dotProduct = Vector3.Dot(userTransform.forward, dirToTarget);

                // 각도 제한이 적용된 판정
                if (dotProduct > angleLimit) // 120도 내에 있음
                {
                    ISkillTarget target = collider.GetComponent<ISkillTarget>();
                    if (target != null)
                    {
                        ApplyDamage(target, damage);
                        ApplyStatusEffect(target, StatusEffectType.Knockback, 1f, knockbackPower);
                    }
                }
            }
        }
    }
}

// 재의손: 지옥분출 (장판 공격)
public class HellfumeSkill : Skill
{
    private float radius = 4f;
    private float duration = 5f;

    public HellfumeSkill()
    {
        id = SkillId.Hellfume;
        displayName = "지옥분출";
        description = "고정 위치에 장판 공격";
        cooldown = 3f;
        damage = 200000f;
    }

    public override void Execute(ISkillUser user)
    {
        if (user == null) return;

        // 애니메이션 설정
        user.SetAnimation("Cast");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetPlayerTransform();
        if (userTransform != null)
        {
            // 장판 위치 계산 (플레이어 전방)
            Vector3 groundPos = userTransform.position + userTransform.forward * radius * 1.5f + new Vector3(0f, 1f, 0f);

            // 장판 객체 생성
            GameObject groundEffect = new GameObject("GroundEffect");
            groundEffect.transform.position = groundPos;

            // 지속 데미지 컴포넌트 추가
            GroundDamageZone damageZone = groundEffect.AddComponent<GroundDamageZone>();
            damageZone.Initialize(damage, radius, duration);

            // 시각 이펙트 생성
            GameObject effectObj = GameObject.Instantiate(
                Resources.Load<GameObject>("Effects/HellfumeEffect"),
                groundPos,
                Quaternion.identity);

            // 이펙트 크기 설정
            effectObj.transform.localScale = new Vector3(radius, 1f, radius);

            // 이펙트를 지속 시간 동안 유지
            GameObject.Destroy(effectObj, duration);

            // 지속 시간 후 객체 제거
            GameObject.Destroy(groundEffect, duration);
        }
    }
}

// 추적자: 천공의 창 (관통 투사체 + 에어본)
public class SkyLanceSkill : Skill
{
    private int penetrationCount = 10;
    private float airborneDuration = 2f;

    public SkyLanceSkill()
    {
        id = SkillId.SkyLance;
        displayName = "천공의 창";
        description = "투사체를 먼 거리로 발사하여 정해진 횟수만큼 관통한다";
        cooldown = 3f;
        damage = 35000f;
    }

    public override void Execute(ISkillUser user)
    {
        if (user == null) return;

        // 애니메이션 설정
        user.SetAnimation("Attack");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetPlayerTransform();
        if (userTransform != null)
        {
            // 투사체 생성 위치
            Vector3 spawnPos = userTransform.position + userTransform.forward * 1f + new Vector3(0f, 1f, 0f);

            // 투사체 객체 생성
            GameObject projectileObj = GameObject.Instantiate(
                Resources.Load<GameObject>("Projectiles/PenetrationProjectile"),
                spawnPos,
                userTransform.rotation);

            // 관통 투사체 컴포넌트 설정
            PenetrationProjectile projectile = projectileObj.GetComponent<PenetrationProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(damage, 20f, penetrationCount);
                projectile.SetStatusEffect(StatusEffectType.Airborne, airborneDuration);
            }
        }
    }
}

// 파편사: 파멸의 궤도 (멀티샷)
public class OrbitOfRuinSkill : Skill
{
    private int shotCount = 5;
    private float shotAngle = 20f;
    public OrbitOfRuinSkill()
    {
        id = SkillId.OrbitOfRuin;
        displayName = "파멸의 궤도";
        description = "먼거리를 정해진 갯수만큼 동시에 투사체를 발사한다";
        cooldown = 2f;
        damage = 150000f;
    }

    public override void Execute(ISkillUser user)
    {
        if (user == null) return;

        // 애니메이션 설정
        user.SetAnimation("Attack");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetPlayerTransform();
        if (userTransform != null)
        {
            // 여러 방향으로 투사체 발사
            for (int i = 0; i < shotCount; i++)
            {
                float angle = i * shotAngle;
                Quaternion rotation = userTransform.rotation * Quaternion.Euler(0, angle, 0);

                // 투사체 생성
                GameObject projectileObj = GameObject.Instantiate(
                    Resources.Load<GameObject>("Projectiles/Projectile"),
                    userTransform.position + new Vector3(0, 1f, 0),
                    rotation);

                // 투사체 컴포넌트 설정
                SkillProjectile projectile = projectileObj.GetComponent<SkillProjectile>();
                if (projectile != null)
                {
                    projectile.Initialize(damage, 15f);
                }
            }
        }
    }
}

// 속사수: 악마의 연타 (속사)
public class InfernalVolleySkill : Skill
{
    private int shotCount = 10;
    private float fireInterval = 0.1f;

    public InfernalVolleySkill()
    {
        id = SkillId.InfernalVolley;
        displayName = "악마의 연타";
        description = "먼 거리의 적까지 빠른 공격속도로 단일 몹 집중 공격";
        cooldown = 3f;
        damage = 100000f;
    }

    public override void Execute(ISkillUser user)
    {
        if (user == null) return;

        // 애니메이션 설정
        user.SetAnimation("Attack");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetTransform();
        if (userTransform != null)
        {
            // 연속 발사 객체 생성
            GameObject rapidFireObj = new GameObject("RapidFire");
            rapidFireObj.transform.position = userTransform.position;
            rapidFireObj.transform.rotation = userTransform.rotation;

            // 연속 발사 컴포넌트 추가
            RapidFireController rapidFire = rapidFireObj.AddComponent<RapidFireController>();
            rapidFire.Initialize(userTransform, damage, shotCount, fireInterval);

            // 모든 발사 완료 후 객체 제거
            GameObject.Destroy(rapidFireObj, shotCount * fireInterval + 0.5f);
        }
    }
}

// 처형자: 지옥의 족쇄 (단일 투사체 + 스턴)
public class HellsnareSkill : Skill
{
    private float stunDuration = 3f;

    public HellsnareSkill()
    {
        id = SkillId.Hellsnare;
        displayName = "지옥의 족쇄";
        description = "투사체를 발사하여 원거리 저격";
        cooldown = 3f;
        damage = 20000f;
    }

    public override void Execute(ISkillUser user)
    {
        if (user == null) return;

        // 애니메이션 설정
        user.SetAnimation("Attack");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetPlayerTransform();
        if (userTransform != null)
        {
            // 투사체 생성 위치
            Vector3 spawnPos = userTransform.position + userTransform.forward * 1f+ new Vector3(0f,1f,0f);

            // 투사체 객체 생성
            GameObject projectileObj = GameObject.Instantiate(
                Resources.Load<GameObject>("Projectiles/StunProjectile"),
                spawnPos,
                userTransform.rotation);

            // 투사체 컴포넌트 설정
            SkillProjectile projectile = projectileObj.GetComponent<SkillProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(damage, 15f);
                projectile.SetStatusEffect(StatusEffectType.Stun, stunDuration);
            }
        }
    }
}

// 망령술사: 광기의 안개 (광역 혼란)
public class MistOfMadnessSkill : Skill
{
    private float radius = 5f;
    private float confusionDuration = 2f;

    public MistOfMadnessSkill()
    {
        id = SkillId.MistOfMadness;
        displayName = "광기의 안개";
        description = "플레이어 주변 모든 적을 공격하며 광역 혼란시킨다";
        cooldown = 3f;
    }

    public override void Execute(ISkillUser user)
    {
        if (user == null) return;

        // 애니메이션 설정
        user.SetAnimation("Cast");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetTransform();
        if (userTransform != null)
        {
            // 원형 이펙트 생성
            GameObject effectObj = GameObject.Instantiate(
                Resources.Load<GameObject>("Effects/MistOfMadnessEffect"),
                userTransform.position,
                Quaternion.identity);

            // 이펙트 크기 설정
            effectObj.transform.localScale = new Vector3(radius, radius, radius);

            // 일정 시간 후 제거
            GameObject.Destroy(effectObj, 3f);

            // 범위 내 적 찾기
            Collider[] colliders = Physics.OverlapSphere(userTransform.position, radius, LayerMask.GetMask("Enemy"));

            // 각 적에게 데미지와 상태 효과 적용
            foreach (var collider in colliders)
            {
                ISkillTarget target = collider.GetComponent<ISkillTarget>();
                if (target != null)
                {
                    ApplyStatusEffect(target, StatusEffectType.Confusion, confusionDuration);
                }
            }
        }
    }
}
