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

public abstract class BaseEntity : WorldObject, IBaseEntity
{
    public abstract BaseModel GetBaseModel();
}
public abstract class BaseEntity<M> : BaseEntity, IBaseEntity<M> where M : BaseModel
{
    public M Model { get; set; }

    public override BaseModel GetBaseModel() => Model;
    public M GetModel() => Model;

    protected virtual void OnDisable() => Model?.Unload();
    protected virtual void OnDestroy() => Model?.Unload();
}