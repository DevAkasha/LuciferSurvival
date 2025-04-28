using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSkillModule : PlayerPart
{
    public Action Roll => ActivateRoll;
    public Action Ghost => ActivateGhost;
    public Action Barrier => ActivateBarrier;
    public Action Exhaust => ActivateExhaust;

    public readonly List<(float cooldown, Action skill)> PlayerSkillList = new();

    [SerializeField] private float rollSpeed = 8f;
    [SerializeField] private float rollDuration = 0.6f;
    [SerializeField] private AnimationCurve rollSpeedCurve;

    private bool IsRolling 
    { 
        get => Model.Flags.GetValue(PlayerStateFlag.Roll); 
        set => Model.Flags.SetValue(PlayerStateFlag.Roll, value);
    }

    private void Awake()
    {
        RegisterSkills();
    }

    private void RegisterSkills()
    {
        PlayerSkillList.Add((16f, Roll));
        PlayerSkillList.Add((60f, Ghost));
        PlayerSkillList.Add((60f, Barrier));
        PlayerSkillList.Add((60f, Exhaust));
    }

    public void ActivateRoll()
    {
        if (!IsRolling)
        {
            StartCoroutine(RollCoroutine());
        }
    }

    private IEnumerator RollCoroutine()
    {
        IsRolling = true;
        float timer = 0f;

        Entity.isStopMove = true;
        Vector3 direction = transform.forward; // 또는 현재 이동 방향

        while (timer < rollDuration)
        {
            transform.position += direction * rollSpeed * Time.deltaTime;

            timer += Time.deltaTime;
            yield return null;
        }

        IsRolling = false;
        Entity.isStopMove = false;
    }

    public void ActivateGhost()
    {
        var effect = EffectManager.Instance.GetModifierEffect(EffectId.Ghost);  // 쓸 스킬

        new EffectApplier(effect)
            .AddTarget(Entity)  // 여기서 this는 IBaseEntity를 구현한 객체여야 함
            .Apply();
    }

    public void ActivateBarrier()
    {
        Entity.BarrierCount = 5;
    }

    public void ActivateExhaust()
    {
        var enemies = FindEnemiesInRange(5f);
        var effect = EffectManager.Instance.GetModifierEffect(EffectId.Exhaust);

        foreach (var enemy in enemies)
        {
            if (effect.IsInterpolated)
            {
                EffectRunner.Instance.RegisterInterpolatedEffect(effect, enemy);
            }
            else
            {
                new EffectApplier(effect)
                    .AddTarget(enemy)
                    .Apply();
            }
        }
    }

    private List<IBaseEntity> FindEnemiesInRange(float range)
    {
        var results = new List<IBaseEntity>();
        var colliders = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Enemy"));

        foreach (var collider in colliders)
        {
            var entity = collider.GetComponent<IBaseEntity>();
            if (entity != null)
            {
                results.Add(entity);
            }
        }

        return results;
    }
}