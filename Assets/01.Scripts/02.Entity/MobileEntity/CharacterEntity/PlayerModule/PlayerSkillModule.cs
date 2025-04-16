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

    private bool isRolling = false;

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
        if (!isRolling)
        {
            StartCoroutine(RollCoroutine());
            Debug.Log($"[Roll] Rolling Start");
        }
    }

    private IEnumerator RollCoroutine()
    {
        isRolling = true;
        float timer = 0f;

        // 무적 상태 진입
        Debug.Log("무적상태 돌입");

        Entity.isStopMove = true;
        Vector3 direction = transform.forward; // 또는 현재 이동 방향

        while (timer < rollDuration)
        {
            transform.position += direction * rollSpeed * Time.deltaTime;

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("무적상태 탈출");
        isRolling = false;
        Entity.isStopMove = false;
    }

    public void ActivateGhost()
    {
        var effect = EffectManager.Instance.GetEffect(EffectId.Ghost);  //쓸 스킬
        var applier = new ModifierApplier(effect).AddTarget(Model);     //적용할 애 설정
        EffectRunner.Instance.ApplyTimedEffect(effect, applier);        //적용

        Debug.Log($"[Ghost] MoveSpeed after buff: {Model.MoveSpeed.Value}");
    }

    public void ActivateBarrier()
    {
        Entity.BarrierCount = 5;
        Debug.Log($"[Barrier] Barrier Start");
    }

    public void ActivateExhaust()
    {
        var enemies = FindEnemiesInRange(5f);
        var effect = EffectManager.Instance.GetEffect(EffectId.Exhaust);

        foreach (var entity in FindEnemiesInRange(5f))
        {
            var model = entity.GetBaseModel();
            var applier = new ModifierApplier(effect).AddTarget(model);
            EffectRunner.Instance.ApplyInterpolatedEffect(effect, applier);
        }

        Debug.Log("[Exhaust] Interpolated Slow Applied");
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