using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : WorldObject, IModelOwner
{
    public abstract BaseModel GetBaseModel();
}

public abstract class BaseEntity<M> : BaseEntity, IModelOwner<M>, IRxCaller where M : BaseModel
{
    public bool IsLogicalCaller => true;

    public bool IsMultiRolesCaller => true;

    public bool IsFunctionalCaller => true;

    public bool IsFullRolesCaller => true;

    public M Model { get; set; }

    public override BaseModel GetBaseModel() => Model;
    public M GetModel() => Model;

    protected virtual void OnDisable() => Model?.Unload();
    protected virtual void OnDestroy() => Model?.Unload();
}