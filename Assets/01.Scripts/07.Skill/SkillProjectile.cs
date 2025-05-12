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

    public virtual void Initialize(float damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;
    }

    public void SetStatusEffect(StatusEffectType type, float duration, float power = 1f)
    {
        this.statusType = type;
        this.statusDuration = duration;
        this.statusPower = power;
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
}