using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDamageZone : MonoBehaviour
{
    private float damage;
    private float radius;
    private float duration;
    private float tickInterval = 1f;
    private float elapsedTime = 0f;

    public void Initialize(float damage, float radius, float duration)
    {
        this.damage = damage;
        this.radius = radius;
        this.duration = duration;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        // 일정 간격으로 데미지 적용
        if (elapsedTime >= tickInterval)
        {
            elapsedTime = 0f;

            // 범위 내 적 찾기
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Enemy"));

            // 각 적에게 데미지 적용
            foreach (var collider in colliders)
            {
                ISkillTarget target = collider.GetComponent<ISkillTarget>();
                if (target != null)
                {
                    target.TakeDamaged(damage);
                }
            }
        }
    }
}