using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireController : MonoBehaviour
{
    private Transform userTransform;
    private float damage;
    private int shotCount;
    private float fireInterval;
    private int currentShot = 0;
    private float timer = 0f;

    public void Initialize(Transform userTransform, float damage, int shotCount, float fireInterval)
    {
        this.userTransform = userTransform;
        this.damage = damage;
        this.shotCount = shotCount;
        this.fireInterval = fireInterval;
    }

    private void Update()
    {
        if (currentShot >= shotCount) return;

        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            timer = 0f;
            FireProjectile();
            currentShot++;
        }
    }

    private void FireProjectile()
    {
        if (userTransform == null) return;

        // 투사체 생성 위치
        Vector3 spawnPos = userTransform.position + userTransform.forward * 1f + new Vector3(0f, 1f, 0f);

        // 투사체 객체 생성
        GameObject projectileObj = GameObject.Instantiate(
            Resources.Load<GameObject>("Projectiles/RapidFireProjectile"),
            spawnPos,
            userTransform.rotation);

        // 투사체 컴포넌트 설정
        SkillProjectile projectile = projectileObj.GetComponent<SkillProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(damage, 20f);
        }
    }
}