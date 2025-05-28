using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;


public class AngelEntity : MobileEntity<AngelModel>, ISkillTarget
{
    [SerializeField] private string rcode;

    [SerializeField] private NavMeshAgent navMesh;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private Outline outline;
    public Transform headPivot;
    public bool AttackCoolTime;

    #region ProxyProperty
    protected override float CurHealth 
    { 
        get => Model.CurHealth.Value; 
        set => Model.CurHealth.SetValue(value, this); 
    }
    private float Atk 
    { 
        get => Model.Atk.Value; 
        set => Model.Atk.SetValue(value, this); 
    }
    private bool IsStun 
    { 
        get => Model.Flags.GetValue(PlayerStateFlag.Stun); 
        set => Model.Flags.SetValue(PlayerStateFlag.Stun, value); 
    }
    private bool IsConfuse 
    { 
        get => Model.Flags.GetValue(PlayerStateFlag.Confuse); 
        set => Model.Flags.SetValue(PlayerStateFlag.Confuse, value);
    }
    private bool IsKnockback 
    { 
        get => Model.Flags.GetValue(PlayerStateFlag.Knockback); 
        set => Model.Flags.SetValue(PlayerStateFlag.Knockback, value); 
    }
    private bool IsDeath 
    { 
        get => Model.Flags.GetValue(PlayerStateFlag.Death); 
        set => Model.Flags.SetValue(PlayerStateFlag.Death, value); 
    }
    private bool IsMove 
    { 
        get => Model.Flags.GetValue(PlayerStateFlag.Move); 
        set => Model.Flags.SetValue(PlayerStateFlag.Move, value); 
    }
    #endregion

    public Transform GetTransform()
    {
        return transform;
    }
    public EnemyType GetEnemyType()
    {
        return Model.EnemyType;
    }

    protected override void SetupModel()
    {
        if (rcode.Equals(string.Empty)) return;

        //rcode를 검사해 EnemyDataSO 중 같은 값의 rcode를 가진 SO의 값을 가져옴
        Model = new AngelModel(DataManager.Instance.GetData<EnemyDataSO>(rcode));
    }

    protected override void AtInit()
    {
        navMesh = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        AttackCoolTime = true;
        Model.Flags.AddListener(PlayerStateFlag.Death, OnDeath);
    }

    public void StopMove()
    {
        if (!navMesh.enabled)
            return;
        IsMove = false;
        rigid.velocity = Vector3.zero;
        navMesh.speed = 0f;
    }

    public void MoveTo(Vector3 target)
    {
        if (navMesh == null || !navMesh.enabled || !navMesh.isOnNavMesh)
            return;

        IsMove = true;
        if (rigid.velocity.sqrMagnitude > 0.01f)
            rigid.velocity = Vector3.zero;

        float sqrDistToCurrentTarget = (navMesh.destination - target).sqrMagnitude;

        if (sqrDistToCurrentTarget > 1.0f)
        {
            navMesh.speed = Model.MoveSpeed.Value;

            if (IsConfuse)
            {
                Vector3 directionToTarget = target - transform.position;
                Vector3 reverseTarget = transform.position - directionToTarget;
                target = reverseTarget;
            }

            navMesh.SetDestination(target);
        }
    }

    public async UniTaskVoid DeshTo(Vector3 target)
    {
        if (navMesh == null || !navMesh.enabled || !navMesh.isOnNavMesh)
            return;

        IsMove = false;
        if (rigid.velocity.sqrMagnitude > 0.01f)
            rigid.velocity = Vector3.zero;

        rigid.freezeRotation = true;
        // 1. 시선은 타겟을 향하도록 고정
        Vector3 disDirection = (target - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(disDirection);

        // 2. Rigidbody 이용해 뒤로 이동 (등을 보이지 않고 후진)
        Vector3 backDirection = -disDirection; // 뒤로
        rigid.velocity = backDirection * 5f; // 후진 속도

        navMesh.SetDestination(target);
        
        FlashRedOutline();

        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), DelayType.DeltaTime, PlayerLoopTiming.Update);

        Vector3 direction = (target - transform.position).normalized;
        Vector3 force = direction.normalized * 15f;

        rigid.velocity = Vector3.zero; // 기존 움직임 제거
        rigid.AddForce(force, ForceMode.Impulse); // 순간적인 밀림

        await UniTask.WaitUntil(() => rigid.velocity.magnitude <= 0.1f,cancellationToken: this.GetCancellationTokenOnDestroy());

        // === [5] 물리 상태 초기화 ===
        rigid.freezeRotation = false;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

    }

    public override void TakeDamaged(float damage)
    {
        base.TakeDamaged(damage);
        if(CurHealth<0f)
            IsDeath = true;
    }

    public void OnAttack(PlayerController player)
    {
        player.Entity.TakeDamaged(Atk);

        UniTaskVoid uniTaskVoid = StartSkillCooldown();
    }

    private void OnDeath(bool isDead)
    {
        if (!isDead) return; 
            navMesh.isStopped = true;
    }

    public async void OnStun(float delayTime)
    {
        IsStun = true;
        rigid.velocity = Vector3.zero;

        await UniTask.Delay(TimeSpan.FromSeconds(delayTime), DelayType.DeltaTime, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());

        IsStun = false;
    }

    public async void OnConfuse(float delayTime)
    {
        IsConfuse = true;

        await UniTask.Delay(TimeSpan.FromSeconds(delayTime), DelayType.DeltaTime, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());

        IsConfuse = false;
    }

    public async void OnKnockBack(float KnockBackDistance, Vector3 targetpos)
    {
        IsKnockback = true;
        Vector3 directionFromTarget = targetpos - transform.position;
        Vector3 force = directionFromTarget.normalized * KnockBackDistance;

        rigid.velocity = Vector3.zero; // 기존 움직임 제거
        rigid.AddForce(force, ForceMode.Impulse); // 순간적인 밀림
        await UniTask.WaitUntil(() => rigid.velocity.magnitude <= 0.01f, cancellationToken: this.GetCancellationTokenOnDestroy());
        IsKnockback = false;
    }

    private async UniTaskVoid StartSkillCooldown()
    {
        if (Model.CoolTime.Value < 0)
            return;

        var token = this.GetCancellationTokenOnDestroy();

        AttackCoolTime = false; // 쿨타임 시작
        await UniTask.Delay(TimeSpan.FromSeconds(Model.CoolTime.Value), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
        AttackCoolTime = true;  // 쿨타임 종료
    }

    public async void FlashRedOutline()
    {
        if (outline == null)
            return;

        outline.OutlineColor = Color.red;
        outline.OutlineWidth = 5f;
        outline.enabled = true;

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.DeltaTime, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());

        outline.enabled = false;
    }

    public void OnRelease()
    {
        if (PlayerManager.Instance.Player.Entity.IsDeath)
            return;

        WaveManager.Instance.KillCount++;
        WaveManager.Instance.CheckKillCount();
        StageManager.Instance.AddSoulCore(Model.RewardCount.Value);
        PoolManager.Instance.Release(this);
    }


    public void ApplyStatusEffect(StatusEffectType effectType, float duration, float power = 1f)
    {
        switch (effectType)
        {
            case StatusEffectType.Slow:
                // 슬로우 효과
                var slowEffect = EffectManager.Instance.GetModifierEffect(EffectId.Slow);
                if (slowEffect != null)
                {
                    new EffectApplier(slowEffect).AddTarget(this).Apply();
                }
                break;

            case StatusEffectType.Knockback:
                // 넉백 효과
                OnKnockBack(power, transform.position - transform.forward);
                break;

            case StatusEffectType.Stun:
                // 스턴 효과
                OnStun(duration);
                break;

            case StatusEffectType.Confusion:
                // 혼란 효과
                OnConfuse(duration);
                break;

            case StatusEffectType.Airborne:
                // 에어본 효과
                OnStun(duration);
                OnKnockBack(power, transform.position + Vector3.up * 3);
                break;
        }
    }
}