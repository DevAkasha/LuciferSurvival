using System;
using System.Threading.Tasks.Sources;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class PlayerEntity : MobileEntity<PlayerModel>
{
    public int BarrierCount;
    
    public bool isStopMove;
    public Vector2 moveInput;
    
    protected Vector3 moveDir;
    protected Vector3 moveVel;
    protected Rigidbody rigid;

    public abstract float MoveSpeed { get; set; }

    private bool IsMove
    {
        get => Model.Flags.GetValue(PlayerStateFlag.Move);
        set => Model.Flags.SetValue(PlayerStateFlag.Move, value);
    }

    protected override void OnInit()
    {
        rigid = GetComponent<Rigidbody>();
    }
    
    protected virtual void FixedUpdate()
    {
        if (!isStopMove && Math.Abs(moveInput.x) + Math.Abs(moveInput.y) > 0.001)
        {
            Move();
            return;
        }
        IsMove = false;
    }

    public override void TakeDamaged(float damage)
    {
        if (BarrierCount > 0)
        {
            BarrierCount--;
            return;
        }

        base.TakeDamaged(damage);
    }

    public virtual void Move()
    {
        moveDir.x = moveInput.x;
        moveDir.y = 0f;
        moveDir.z = moveInput.y;
        transform.LookAt(transform.position + moveDir);
        moveVel = moveDir * MoveSpeed;
        rigid.MovePosition(transform.position + moveVel * Time.fixedDeltaTime);
        IsMove = true;
    }
}