using System.Collections.Generic;
using UnityEngine;

public class PenetrationProjectile : SkillProjectile
{
    private int penetrationCount;
    private int currentPenetrations = 0;
    private HashSet<Collider> hitColliders = new HashSet<Collider>();

    public void Initialize(float damage, float speed, int penetrationCount, bool usePriority, EnemyType targetPriority = EnemyType.standard)
    {
        base.Initialize(damage, speed, usePriority, targetPriority);
        this.penetrationCount = penetrationCount;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        // 이미 타격한 대상은 무시
        if (hitColliders.Contains(other)) return;

        ISkillTarget target = other.GetComponent<ISkillTarget>();
        if (target != null)
        {
            // 우선순위 기반 충돌 처리 (옵션)
            if (usePriority && !ShouldDamageTarget(other))
            {
                return; // 우선순위가 더 낮은 대상은 무시
            }

            // 타격 목록에 추가
            hitColliders.Add(other);

            // 데미지 적용
            target.TakeDamaged(damage);

            // 상태 효과 적용
            if (statusType != StatusEffectType.None && statusDuration > 0)
            {
                target.ApplyStatusEffect(statusType, statusDuration, statusPower);
            }

            // 관통 카운트 증가
            currentPenetrations++;

            // 최대 관통 횟수 도달 시 제거
            if (currentPenetrations >= penetrationCount)
            {
                Destroy(gameObject);
            }
        }
    }
}
