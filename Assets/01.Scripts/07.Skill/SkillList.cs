using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleshGrinderSkill : Skill //데미지, 공격사이즈, 
{
    private float radius;
    private float slowDuration = 3f;

    private float[] atkArray = new float[5] {10f,30f,60f,120f,240f}; //7만 60*3 초당 180으로 7만
    private float[] radiusArray = new float[5] { 4f, 4f, 4.5f, 5f, 5.5f };

    public FleshGrinderSkill(eUnitGrade unitGrade)
    {
        id = SkillId.FleshGrinder;
        displayName = "고기 갈기기";
        description = "주변 360도를 공격하는 범위 밀집 공격";
        cooldown = 2f;
        damage = atkArray[(int)unitGrade-1];
        radius = radiusArray[(int)unitGrade - 1];
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
                    damage = (UnityEngine.Random.Range(0f, 100f) < criticalChance) ? damage * criticalRatio : damage;
                    ApplyDamage(target, damage);
                    ApplyStatusEffect(target, StatusEffectType.Slow, slowDuration);
                }
            }
        }
    }
}

// 파쇄자: 분쇄 강타 (전방 범위 공격 + 넉백)
public class CrushBreakerSkill : Skill // 공격력, 공격범위, 
{
    private float radius;
    private float knockbackPower = 3f;
    private float angleLimit = 0.5f; // cos(60도) ≈ 0.5, 즉 전방 120도 범위

    private float[] atkArray = new float[5] { 15f, 40f, 80f, 160f, 320f }; 
    private float[] radiusArray = new float[5] { 4f, 4f, 5f, 5.5f, 6f };

    public CrushBreakerSkill(eUnitGrade unitGrade)
    {
        id = SkillId.CrushBreaker;
        displayName = "분쇄 강타";
        description = "무기를 휘두르며 전방 120도 강타";
        cooldown = 3f;
        damage = atkArray[(int)unitGrade - 1];
        radius = radiusArray[(int)unitGrade - 1];
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
                        damage = (UnityEngine.Random.Range(0f, 100f) < criticalChance) ? damage * criticalRatio : damage;
                        ApplyDamage(target, damage);
                        ApplyStatusEffect(target, StatusEffectType.Knockback, 1f, knockbackPower);
                    }
                }
            }
        }
    }
}

// 재의손: 지옥분출 (장판 공격)
public class HellfumeSkill : Skill // 공격력, 공격범위, 상하좌우, 
{
    private float radius;
    private float duration;
    private int posCount;

    private Vector3[] groundPosArray = new Vector3[4];

    private float[] atkArray = new float[5] { 10f, 20f, 60f, 120f, 240f };
    private float[] radiusArray = new float[5] { 1f, 1.5f, 2f, 2f, 2.5f };
    private float[] durationArray = new float[5] { 3f, 4f, 5f, 5f, 6f };
    private int[] posCountArray = new int[5] { 1, 2, 2, 3, 4 };

    public HellfumeSkill(eUnitGrade unitGrade)
    {
        id = SkillId.Hellfume;
        displayName = "지옥분출";
        description = "고정 위치에 장판 공격";
        cooldown = 2f;
        damage = atkArray[(int)unitGrade - 1];
        radius = radiusArray[(int)unitGrade - 1];
        duration = durationArray[(int)unitGrade - 1];
        posCount = posCountArray[(int)unitGrade - 1];
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
            groundPosArray[0] = userTransform.position + userTransform.forward * radius * 1.5f + new Vector3(0f, 1f, 0f);
            groundPosArray[1] = userTransform.position + userTransform.right * radius * 1.5f + new Vector3(0f, 1f, 0f);
            groundPosArray[2] = userTransform.position + userTransform.forward * radius * -1.5f + new Vector3(0f, 1f, 0f);
            groundPosArray[3] = userTransform.position + userTransform.right * radius * -1.5f + new Vector3(0f, 1f, 0f);

            for(int i = 0; i<posCount; i++)
            {  
                // 장판 객체 생성
                GameObject groundEffect = new GameObject("GroundEffect");
                groundEffect.transform.position = groundPosArray[i];

                // 지속 데미지 컴포넌트 추가
                GroundDamageZone damageZone = groundEffect.AddComponent<GroundDamageZone>();
                damageZone.Initialize(damage, radius, duration);

                // 시각 이펙트 생성
                GameObject effectObj = GameObject.Instantiate(
                    Resources.Load<GameObject>("Effects/HellfumeEffect"),
                    groundPosArray[i],
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
}

// 추적자: 천공의 창 (관통 투사체 + 에어본)
public class SkyLanceSkill : TargetedSkill // 공격력, 카운트
{
    private int penetrationCount;
    private float airborneDuration = 2f;

    private float[] atkArray = new float[5] { 40f, 80f, 150f, 240f, 480f };
    private int[] penetrationCountArray = new int[5] { 13, 14, 16, 18, 20 };

    public SkyLanceSkill(eUnitGrade unitGrade)
    {
        id = SkillId.SkyLance;
        displayName = "천공의 창";
        description = "투사체를 먼 거리로 발사하여 정해진 횟수만큼 관통한다";
        cooldown = 2f;
        damage = atkArray[(int)unitGrade-1];
        penetrationCount = penetrationCountArray[(int)unitGrade - 1];
    }

    public override void ExecuteWithTarget(ISkillUser user, Transform target, EnemyType targetType)
    {
        if (user == null || target == null) return;

        // 애니메이션 설정
        user.SetAnimation("Attack");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetTransform();
        if (userTransform != null)
        {
            // 투사체 생성 위치
            Vector3 spawnPos = userTransform.position + userTransform.forward * 1f + new Vector3(0f, 1f, 0f);

            Vector3 directionToTarget = target.position - userTransform.position;
            if (directionToTarget == Vector3.zero) return;

            Quaternion rotation = Quaternion.LookRotation(directionToTarget);

            // 투사체 객체 생성
            GameObject projectileObj = GameObject.Instantiate(
                Resources.Load<GameObject>("Projectiles/PenetrationProjectile"),
                spawnPos,
                rotation);

            // 관통 투사체 컴포넌트 설정
            PenetrationProjectile projectile = projectileObj.GetComponent<PenetrationProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(damage, 20f, penetrationCount, false, targetType);
                projectile.SetStatusEffect(StatusEffectType.Airborne, airborneDuration);
            }
        }
    }
}

// 파편사: 파멸의 궤도 (멀티샷)
public class OrbitOfRuinSkill : TargetedSkill //공격력, 샷카운트
{
    private int shotCount;
    private float shotAngle = 20f;

    private float[] atkArray = new float[5] { 40f, 100f, 150f, 250f, 480f };
    private int[] shotCountArray = new int[5] { 3, 4, 5, 6, 7 };

    public OrbitOfRuinSkill(eUnitGrade unitGrade)
    {
        id = SkillId.OrbitOfRuin;
        displayName = "파멸의 궤도";
        description = "먼거리를 정해진 갯수만큼 동시에 투사체를 발사한다";
        cooldown = 2f;
        damage = atkArray[(int)unitGrade - 1];
        shotCount = shotCountArray[(int)unitGrade - 1];
    }

    public override void ExecuteWithTarget(ISkillUser user, Transform target, EnemyType targetType)
    {
        if (user == null || target == null) return;

        // 애니메이션 설정
        user.SetAnimation("Attack");

        Transform userTransform = user.GetTransform();
        if (userTransform != null)
        {
            // 타겟 방향 계산
            Vector3 directionToTarget = target.position - userTransform.position;
            if (directionToTarget == Vector3.zero) return;

            // 기본 방향 설정
            Quaternion baseRotation = Quaternion.LookRotation(directionToTarget);

            // 여러 방향으로 투사체 발사
            for (int i = 0; i < shotCount; i++)
            {
                // 기본 각도를 중심으로 -shotAngle*2 ~ +shotAngle*2 범위에 분포
                float angle = -shotAngle * (shotCount - 1) / 2 + i * shotAngle;
                Quaternion rotation = baseRotation * Quaternion.Euler(0, angle, 0);

                // 발사 위치
                Vector3 spawnPos = userTransform.position + Vector3.up * 1f;

                // 투사체 생성
                GameObject projectileObj = GameObject.Instantiate(
                    Resources.Load<GameObject>("Projectiles/Projectile"),
                    spawnPos,
                    rotation);

                // 투사체 컴포넌트 설정
                SkillProjectile projectile = projectileObj.GetComponent<SkillProjectile>();
                if (projectile != null)
                {
                    projectile.Initialize(damage, 15f,true);
                }
            }
        }
    }
}

// 속사수: 악마의 연타 (속사)
public class InfernalVolleySkill : TargetedSkill //공격력, 샷카운트, 
{
    private int shotCount = 10;
    private float fireInterval = 0.1f;

    private float[] atkArray = new float[5] { 15f, 30f, 80f, 100f, 200f };
    private float[] cooldowntArray = new float[5] { 3f, 2f, 1.8f, 1.5f, 1.5f };

    public InfernalVolleySkill(eUnitGrade unitGrade)
    {
        id = SkillId.InfernalVolley;
        displayName = "악마의 연타";
        description = "먼 거리의 적까지 빠른 공격속도로 단일 몹 집중 공격";
        cooldown = cooldowntArray[(int)unitGrade-1];
        damage = atkArray[(int)unitGrade - 1];
    }

    public override void ExecuteWithTarget(ISkillUser user, Transform target, EnemyType targetType)
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

            Vector3 direction = target.position - userTransform.position;
            if (direction != Vector3.zero)
                rapidFireObj.transform.rotation = Quaternion.LookRotation(direction);

            // 연속 발사 컴포넌트 추가
            RapidFireController rapidFire = rapidFireObj.AddComponent<RapidFireController>();
            rapidFire.InitializeWithTarget(userTransform, target, damage, shotCount, fireInterval, targetType);

            // 모든 발사 완료 후 객체 제거
            GameObject.Destroy(rapidFireObj, shotCount * fireInterval + 0.5f);
        }
    }
}

// 처형자: 지옥의 족쇄 (단일 투사체 + 스턴)
public class HellsnareSkill : TargetedSkill //공격력
{
    private float stunDuration = 0.5f;

    private float[] atkArray = new float[5] { 80f, 150f, 200f, 300f, 480f };

    public HellsnareSkill(eUnitGrade unitGrade)
    {
        id = SkillId.Hellsnare;
        displayName = "지옥의 족쇄";
        description = "투사체를 발사하여 원거리 저격";
        cooldown = 3f;
        damage = atkArray[(int)unitGrade - 1];
    }

    public override void ExecuteWithTarget(ISkillUser user, Transform target, EnemyType targetType)
    {
        if (user == null || target == null) return;

        // 애니메이션 설정
        user.SetAnimation("Attack");

        // 스킬 이펙트 생성
        Transform userTransform = user.GetPlayerTransform();
        if (userTransform != null)
        {
            // 투사체 생성 위치
            Vector3 direction = target.position - userTransform.position;
            if (direction == Vector3.zero) return;

            Quaternion rotation = Quaternion.LookRotation(direction);

            Vector3 spawnPos = userTransform.position + direction.normalized * 1f + new Vector3(0f, 1f, 0f);
            // 투사체 객체 생성
            GameObject projectileObj = GameObject.Instantiate(
                       Resources.Load<GameObject>("Projectiles/StunProjectile"),
                       spawnPos,
                       rotation);


            // 투사체 컴포넌트 설정
            SkillProjectile projectile = projectileObj.GetComponent<SkillProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(damage, 15f,true, targetType);
                projectile.SetStatusEffect(StatusEffectType.Stun, stunDuration);
            }
        }
    }
}

// 망령술사: 광기의 안개 (광역 혼란)
public class MistOfMadnessSkill : Skill //공격력 공격범위
{
    private float radius;
    private float confusionDuration = 0.5f;

    private float[] atkArray = new float[5] { 10f, 30f, 60f, 120f, 240f };
    private float[] radiusArray = new float[5] { 4f, 4f, 4.5f, 5f, 5.5f };

    public MistOfMadnessSkill(eUnitGrade unitGrade)
    {
        id = SkillId.MistOfMadness;
        displayName = "광기의 안개";
        description = "플레이어 주변 모든 적을 공격하며 광역 혼란시킨다";
        cooldown = 3f;
        damage = atkArray[(int)unitGrade - 1];
        radius = radiusArray[(int)unitGrade - 1];
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
