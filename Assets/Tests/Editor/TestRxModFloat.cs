using System.Collections.Generic;
using NUnit.Framework;

public enum TestEffectId
{
    BuffA,
    DebuffB
}

public class RxModTests
{
    private class TestModel : BaseModel
    {
        public RxModFloat Power = new(100f);

        public override IEnumerable<IModifiable> GetModifiables()
        {
            yield return Power;
        }
    }

    [Test]
    public void ApplyModifierEffect_ModifiesValueCorrectly()
    {
        // 초기 모델 생성
        var model = new TestModel();

        // ModifierEffect 생성 및 등록
        var effect = new ModifierEffect(TestEffectId.BuffA)
            .Add(ModifierType.OriginAdd, 50f) // +50
            .Add(ModifierType.AddMultiplier, 0.5f) // ×1.5
            .Add(ModifierType.Multiplier, 2f); // ×2
        EffectManager.Instance.Register(effect);

        // ModifierApplier로 모델에 적용
        var applier = new ModifierApplier(effect).AddTarget(model);
        applier.Apply();

        // 기대값 계산: ((100 + 50) * 1.5) * 2 = 450
        Assert.AreEqual(450f, model.Power.Value);
    }

    [Test]
    public void RemoveModifierEffect_RestoresOriginalValue()
    {
        var model = new TestModel();

        var effect = new ModifierEffect(TestEffectId.DebuffB)
            .Add(ModifierType.OriginAdd, -20f);
        EffectManager.Instance.Register(effect);

        var applier = new ModifierApplier(effect).AddTarget(model);
        applier.Apply();

        Assert.AreEqual(80f, model.Power.Value);

        applier.Remove();

        Assert.AreEqual(100f, model.Power.Value);
    }
}