using System.Collections.Generic;
using UnityEngine;

public class EnhancedProjectile : Projectile
{
    public bool isPenetrating = false;
    public int penetrationCount = 1;
    public float aoeRadius = 0f;

    private int currentPenetrations = 0;
    private HashSet<Collider> hitColliders = new HashSet<Collider>();
    private ProjectileStatusEffect statusEffect;

    private void Awake()
    {
        statusEffect = GetComponent<ProjectileStatusEffect>();
        if (statusEffect == null)
        {
            statusEffect = gameObject.AddComponent<ProjectileStatusEffect>();
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (hitColliders.Contains(other)) return;

        var entity = other.GetComponent<AngelEntity>();
        if (entity == null) return;

        // 타격 대상 추가
        hitColliders.Add(other);

        // 데미지 적용
        entity.TakeDamaged(damage);

        // 상태 효과 적용
        if (statusEffect != null)
        {
            statusEffect.ApplyEffects(entity);
        }

        // 범위 공격 처리
        if (aoeRadius > 0)
        {
            var colliders = Physics.OverlapSphere(transform.position, aoeRadius, 1 << LayerMask.NameToLayer("Enemy"));
            foreach (var col in colliders)
            {
                if (col == other || hitColliders.Contains(col)) continue;

                var aoeEntity = col.GetComponent<AngelEntity>();
                if (aoeEntity != null)
                {
                    hitColliders.Add(col);
                    aoeEntity.TakeDamaged(damage);

                    if (statusEffect != null)
                    {
                        statusEffect.ApplyEffects(aoeEntity);
                    }
                }
            }
        }

        // 관통 처리
        if (isPenetrating)
        {
            currentPenetrations++;
            if (currentPenetrations >= penetrationCount)
            {
                PoolManager.Instance.Release(this);
            }
        }
        else
        {
            PoolManager.Instance.Release(this);
        }
    }
}