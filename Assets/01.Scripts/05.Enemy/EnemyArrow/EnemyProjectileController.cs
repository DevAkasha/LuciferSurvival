using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    [SerializeField]
    private Projectile projectile;

    [SerializeField]
    private LayerMask layerMask;

    private Vector3 direction;

    private void Start()
    {
        if (projectile?.Target != null)
        {
            direction = (projectile.Target.position - transform.position).normalized;
            direction.y = 0f;
        }
    }

    private void OnEnable()
    {
        projectile.OnRelease();
    }

    void Update()
    {
        ProjectileMove();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerMask)
        {
            other.GetComponent<PlayerEntity>()?.TakeDamaged(projectile.damage);
            Destroy(gameObject);
        }
    }

    private void ProjectileMove()
    {
        transform.position += direction * projectile.speed * Time.deltaTime;
    }
}
