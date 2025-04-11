using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSkillModule : PlayerPart
{
    public void ActivateGhost()
    {
        var effect = EffectManager.Instance.GetEffect(EffectId.Ghost);  //쓸 스킬
        var applier = new ModifierApplier(effect).AddTarget(Model);     //적용할 애 설정
        EffectRunner.Instance.ApplyTimedEffect(effect, applier);        //적용

        Debug.Log($"[Ghost] MoveSpeed after buff: {Model.MoveSpeed.Value}");
    }

    protected override void Start()
    {
        base.Start();
        ActivateGhost();
    }
}