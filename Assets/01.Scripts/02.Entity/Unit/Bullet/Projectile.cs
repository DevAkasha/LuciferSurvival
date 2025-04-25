using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : ProjectileBase
{
    public Transform Target { get; set; }

    public void Init(float speed, int typeCount, float damage, Transform target)
    {
        this.speed = speed;
        this.typeCount = typeCount;
        this.damage = damage;
        Target = target;
    }

    public void OnShoot()
    {
        
    }
}
