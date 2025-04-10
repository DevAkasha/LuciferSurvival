
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRunner : Singleton<EffectRunner>
{
    public void ApplyTimedEffect(ModifierEffect effect, ModifierApplier applier)
    {
        Debug.Log($"[EffectRunner] <color=cyan>Effect START</color> - {effect.Key}, Duration: {effect.Duration}s");
        applier.Apply();

        if (effect.Mode == EffectApplyMode.Timed)
            StartCoroutine(RemoveAfter(effect.Duration, applier));
    }

    private IEnumerator RemoveAfter(float seconds, ModifierApplier applier)
    {
        Debug.Log($"[EffectRunner] <color=yellow>Effect END</color> - 리무브 코루틴 시작함: {seconds}sec");
        yield return new WaitForSeconds(seconds);
        Debug.Log($"[EffectRunner] <color=yellow>Effect END</color> - 리무브 대기 종료");
        applier.Remove();
        Debug.Log($"[EffectRunner] <color=yellow>Effect END</color> - 이펙트 종료됨");
    }
}