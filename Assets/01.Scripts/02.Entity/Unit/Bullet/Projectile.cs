using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    public async void OnRelease()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(8), DelayType.DeltaTime, PlayerLoopTiming.Update);
        //PoolManager.Instance.Release(this);
        Destroy(gameObject);
    }

    public void OnShoot()
    {
        
    }
}
