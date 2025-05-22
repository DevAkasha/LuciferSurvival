using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectileController : MonoBehaviour
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
            other.GetComponent<AngelEntity>()?.TakeDamaged(projectile.damage);
        }
    }

    private void ProjectileMove()
    {
        transform.position += direction * projectile.speed * Time.deltaTime;
    }
}
