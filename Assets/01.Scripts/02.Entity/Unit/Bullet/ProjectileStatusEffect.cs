using System.Collections.Generic;
using UnityEngine;

public class ProjectileStatusEffect : MonoBehaviour
{
    public float slowAmount = 0f;
    public float slowDuration = 0f;

    public float knockbackPower = 0f;

    public float stunDuration = 0f;

    public float confuseDuration = 0f;

    public float airborneDuration = 0f;

    private HashSet<AngelEntity> hitEntities = new HashSet<AngelEntity>();

    // 상태 효과 적용
    public void ApplyEffects(AngelEntity entity)
    {
        if (entity == null || hitEntities.Contains(entity)) return;

        hitEntities.Add(entity);

        // 슬로우
        if (slowAmount > 0 && slowDuration > 0)
        {
            var slowEffect = EffectManager.Instance.GetModifierEffect(EffectId.Slow);
            if (slowEffect != null)
            {
                new EffectApplier(slowEffect).AddTarget(entity).Apply();
            }
        }

        // 넉백
        if (knockbackPower > 0)
        {
            entity.OnKnockBack(knockbackPower, transform.position);
        }

        // 스턴
        if (stunDuration > 0)
        {
            entity.OnStun(stunDuration);
        }

        // 혼란
        if (confuseDuration > 0)
        {
            entity.OnConfuse(confuseDuration);
        }

        // 에어본
        if (airborneDuration > 0)
        {
            // 에어본은 위로 띄우고 스턴과 같이 사용
            entity.OnStun(airborneDuration);
            entity.OnKnockBack(3f, entity.transform.position + Vector3.up * 3);
        }
    }
}