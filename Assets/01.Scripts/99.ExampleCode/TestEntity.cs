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

    public override float Hp
    {
        get => model.Hp.Value;
        set => model.Hp.SetValue(value);
    }

    private void Start()
    {
        Damaged(30);
        Debug.Log($"플레이어 체력 : {Hp}");
    }


}
