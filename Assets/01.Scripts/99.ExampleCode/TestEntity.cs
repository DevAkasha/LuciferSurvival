using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEntity : MobileEntity<TestModel>
{
    public TestModel model;

    protected override void SetupModel()
    {
        model = new TestModel();
    }

    protected override float CurHealth
    {
        get => model.Hp.Value;
        set => model.Hp.SetValue(value, this);
    }

    private void Start()
    {
        TakeDamaged(30);
        Debug.Log($"플레이어 체력 : {CurHealth}");
    }


}
