using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseEntity  // GetComponent용 엔티티 인터페이스
{
    BaseModel GetBaseModel();
}

public interface IBaseEntity<M> : IBaseEntity where M : BaseModel // 모델을 소유하는 엔티티 인터페이스
{
    public M Model { get; set; }

    public M GetModel(); //현재 모델 반환
}

public abstract class BaseEntity<M> : WorldObject, IBaseEntity<M> where M : BaseModel
{
    public M Model { get; set; }

    protected virtual void OnDisable()
    {
        Model.Unload(); // 모델의 모든 리액티브 연결 해제
    }

    protected virtual void OnDestroy()
    {
        Model.Unload(); // 모델의 모든 리액티브 연결 해제
    }

    public virtual M GetModel() // 현재 모델 반환
    {
        return Model;
    }

    BaseModel IBaseEntity.GetBaseModel() => Model;
}
