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

    // Start is called before the first frame update
    void Start()
    {
        
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
        Vector3 direction;
        direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        transform.position += direction * projectile.speed * Time.deltaTime;
    }
}
