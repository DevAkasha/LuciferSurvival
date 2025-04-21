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

    [SerializeField]   //���߿� ����(��Ʈ�ѷ� Ÿ�� �ʱ�ȭ �Լ� ���� �ʿ� �־��)
    private Transform target;

    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = (target.position - transform.position).normalized;
        direction.y = 0f;
    }

    void Update()
    {
        ProjectileMove();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerMask)
        {
            other.GetComponent<EnemyEntity>()?.TakeDamaged(projectile.damage);
        }
    }

    private void ProjectileMove()
    {
        transform.position += direction * projectile.speed * Time.deltaTime;
    }
}
