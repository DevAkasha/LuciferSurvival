using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BulletType
{
    Default,
    Piercing,
    Splash,
    Reflection,
    Guided,
}

public class BulletEntity : BaseEntity
{
    private BulletType bulletType;

    [SerializeField]
    private LayerMask targetLayer;

    public float Speed { get; set; }

    public Transform Target { get; set; }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void SetupModels()
    {
        bulletType = BulletType.Default;
    }

    public void Move()
    {
        if (bulletType != BulletType.Guided)
        {

        }

        //테스트용
        Speed = 7f;

        transform.position += Vector3.forward * Time.deltaTime * Speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((targetLayer & (1 << collision.gameObject.layer)) != 0)
        {
            Destroy(GetComponentInParent<GameObject>().gameObject);
        }
    }

    public void ChaseEnemy()
    {

    }
}
