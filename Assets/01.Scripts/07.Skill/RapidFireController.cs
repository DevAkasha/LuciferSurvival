using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireController : MonoBehaviour
{
    private Transform userTransform;
    private Transform targetTransform;
    private EnemyType tagetType = EnemyType.none;
    private float damage;
    private int shotCount;
    private float fireInterval;
    private int currentShot = 0;
    private float timer = 0f;
    
    public void InitializeWithTarget(Transform userTransform, Transform targetTransform, float damage, int shotCount, float fireInterval, EnemyType tagetType)
    {
        this.userTransform = userTransform;
        this.targetTransform = targetTransform;
        this.damage = damage;
        this.shotCount = shotCount;
        this.fireInterval = fireInterval;
        this.tagetType = tagetType;
    }

    private void Update()
    {
        if (currentShot >= shotCount || targetTransform == null) return;

        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            timer = 0f;
            FireProjectileAtTarget();
            currentShot++;
        }
    }

    private void FireProjectileAtTarget()
    {
        if (userTransform == null || targetTransform == null) return;

        // 타겟 방향 계산
        Vector3 direction = targetTransform.position - userTransform.position;
        if (direction == Vector3.zero) return;

        Quaternion rotation = Quaternion.LookRotation(direction);

        // 투사체 생성 위치
        Vector3 spawnPos = userTransform.position + direction.normalized * 1f + new Vector3(0f, 1f, 0f);

        // 투사체 객체 생성
        GameObject projectileObj = GameObject.Instantiate(
            Resources.Load<GameObject>("Projectiles/RapidFireProjectile"),
            spawnPos,
            rotation);

        // 투사체 컴포넌트 설정
        SkillProjectile projectile = projectileObj.GetComponent<SkillProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(damage, 20f,true, tagetType);
        }
    }
}