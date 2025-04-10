using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class PlayerEntity : MobileEntity<PlayerModel>
{
    public Vector2 moveInput;
    protected Vector3 moveDir;
    protected Vector3 moveVel;
    protected Rigidbody rigid;

    public abstract float MoveSpeed { get; set; }

    protected override void OnInit()
    {
        rigid = GetComponent<Rigidbody>();
    }
    
    protected virtual void FixedUpdate()
    {
        Move();
    }

    public virtual void Move()
    {
        moveDir.x = moveInput.x;
        moveDir.y = 0f;
        moveDir.z = moveInput.y;
        moveVel = moveDir * MoveSpeed;
        rigid.MovePosition(transform.position + moveVel * Time.fixedDeltaTime);
    }
}