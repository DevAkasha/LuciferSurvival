using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePart : WorldObject 
{
    public void CallInit()
    {
        SetupModel();
        AtInit();
    }

    protected abstract void SetupModel();

    protected virtual void AtInit() { }
}

public abstract class BasePart<E, M> : BasePart where E : IBaseEntity<M> where M : BaseModel
{
    protected E Entity { get; set; }
    protected M Model { get; set; }
<<<<<<< Updated upstream
    protected virtual void Start() // Unity Start 함수
    {
        Model ??= Entity.GetModel();
        OnStart(); // Unity Start 함수 후크
    }
    protected override void SetupModel() // 모델 초기화
    {
        Entity = (E)GetComponent<IBaseEntity<M>>();
=======

    protected override void SetupModel() // 모델 초기화
    {
        Entity = (E)GetComponentInParent<IModelOwner<M>>();
>>>>>>> Stashed changes
        Model = Entity.GetModel();
    }
}
