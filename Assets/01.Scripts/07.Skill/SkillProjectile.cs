using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    protected float damage;
    protected float speed;
    protected float lifetime = 5f;
    protected StatusEffectType statusType = StatusEffectType.None;
    protected float statusDuration = 0f;
    protected float statusPower = 1f;

    private float timer = 0f;

    protected bool usePriority = false; // 우선순위 적용 여부
    protected EnemyType targetPriority = EnemyType.standard; // 대상 우선순위

    public virtual void Initialize(float damage, float speed, bool usePriority, EnemyType targetPriority = EnemyType.standard)
    {
        this.damage = damage;
        this.speed = speed;
        this.usePriority = usePriority;
        this.targetPriority = targetPriority;
    }

    public void SetStatusEffect(StatusEffectType type, float duration, float power = 1f)
    {
        this.statusType = type;
        this.statusDuration = duration;
        this.statusPower = power;
    }
    public void SetPriorityTargeting(bool respect, EnemyType targetType)
    {
        usePriority = respect;
        targetPriority = targetType;
    }

    protected virtual void Update()
    {
        // 전방으로 이동
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 수명 체크
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        ISkillTarget target = other.GetComponent<ISkillTarget>();
        if (target != null)
        {
            // 우선순위 기반 충돌 처리 (옵션)
            if (usePriority && !ShouldDamageTarget(other))
            {
                return; // 우선순위가 더 낮은 대상은 무시
            }

            // 데미지 적용
            target.TakeDamaged(damage);

            // 상태 효과 적용
            if (statusType != StatusEffectType.None && statusDuration > 0)
            {
                target.ApplyStatusEffect(statusType, statusDuration, statusPower);
            }

            // 투사체 제거
            Destroy(gameObject);
        }
    }
    protected bool ShouldDamageTarget(Collider other)
    {
        // AngelEntity 확인
        AngelEntity angelEnemy = other.GetComponent<AngelEntity>();
        if (angelEnemy != null && angelEnemy.Model != null)
        {
            EnemyType enemyType = angelEnemy.Model.EnemyType;
            // 투사체의 대상 우선순위보다 적의 우선순위가 높으면 데미지 적용하지 않음
            return EnemyPrioritySystem.HasHigherPriority(enemyType, targetPriority);
        }

        // BossEntity 확인
        BossEntity bossEnemy = other.GetComponent<BossEntity>();
        if (bossEnemy != null)
        {
            // 보스는 항상 boss 타입으로 간주
            return EnemyPrioritySystem.HasHigherPriority(EnemyType.boss, targetPriority);
        }

        // 타입을 확인할 수 없으면 기본적으로 데미지 적용
        return false;
    }
}