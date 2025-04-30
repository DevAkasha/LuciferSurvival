using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePart : WorldObject { }

public abstract class BasePart<E, M> : BasePart, IRxCaller where E : IModelOwner<M>  where M : BaseModel
{
    bool IRxCaller.IsLogicalCaller => true;
    bool IRxCaller.IsMultiRolesCaller => true;
    bool IRxCaller.IsFunctionalCaller => true;

    protected E Entity { get; set; }
    protected M Model { get; set; }

    protected virtual void Start() // Unity Start 함수
    {
        Model ??= Entity.GetModel();
        OnStart(); // Unity Start 함수 후크
    }
    protected override void SetupModel() // 모델 초기화
    {
        Entity = (E)GetComponent<IModelOwner<M>>();
        Model = Entity.GetModel();
    }

    protected virtual void OnStart() { } // Unity Start 함수 후크
}
