using System;
using System.Threading.Tasks.Sources;
using UnityEngine;


[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class PlayerEntity : MobileEntity<PlayerModel>
{
    [SerializeField] private string rcode;
    [SerializeField] protected Rigidbody rigid;

    public int BarrierCount;
    
    public bool isStopMove;
    public Vector2 moveInput;
    
    protected Vector3 moveDir;
    protected Vector3 moveVel;

    protected override void SetupModel()
    {
        //if (rcode.Equals(string.Empty)) return; 나중에 데이터 로드할 때 사용

        Model = new PlayerModel();
    }

    protected override float CurHealth
    {
        get => Model.CurHealth.Value;
        set => Model.CurHealth.SetValue(value, this);
    }

    public float MoveSpeed
    {
        get => Model.MoveSpeed.Value;
        set => Model.MoveSpeed.SetValue(value, this);
    }

    private bool IsMove
    {
        get => Model.Flags.GetValue(PlayerStateFlag.Move);
        set => Model.Flags.SetValue(PlayerStateFlag.Move, value);
    }
    private bool IsDeath
    {
        get => Model.Flags.GetValue(PlayerStateFlag.Death);
        set => Model.Flags.SetValue(PlayerStateFlag.Death, value);
    }
    protected override void AtInit()
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
        if (CurHealth < 0f) 
            IsDeath = true;
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