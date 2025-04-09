using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerModel: BaseModel
{
    public RxVar<float> Hp = new(100);
    public RxVar<float> MoveSpeed = new(4);

    public RxVar<float> RollCoolTime = new(15f);

    public RxVar<float> GhostCoolTime = new(60f);

    public RxVar<float> BarrierCoolTime = new(60f);

    public RxVar<float> ExhaustCoolTime = new(60f);
}
public class SkillCool
{
    private float cooldownTime;
    public float lastUsedTime = -Mathf.Infinity;

    public SkillCool(float cooldown)
    {
        cooldownTime = cooldown;
    }

    public bool IsReady => Time.time >= lastUsedTime + cooldownTime;
    public float RemainingTime => Mathf.Max(0, lastUsedTime + cooldownTime - Time.time);
    public float CooldownProgress => Mathf.Clamp01((Time.time - lastUsedTime) / cooldownTime);

    public bool TryUse()
    {
        if (!IsReady) return false;
        lastUsedTime = Time.time;
        return true;
    }
}