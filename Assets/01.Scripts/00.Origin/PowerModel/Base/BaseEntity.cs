using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : WorldObject, IModelOwner, IRxCaller
{
    public bool IsLogicalCaller => true;

    public bool IsMultiRolesCaller => true;

    public bool IsFunctionalCaller => true;

    public abstract BaseModel GetBaseModel();
}

public abstract class BaseEntity<M> : BaseEntity, IModelOwner<M> where M : BaseModel
{
    public M Model { get; set; }

    public override BaseModel GetBaseModel() => Model;

    public M GetModel() => Model;

    public void CallInit()
    {

        SetupModel();
        AtInit();
    }

    protected abstract void SetupModel();

    protected virtual void AtInit() { }

    
    public virtual void AtDisable() { }
    public virtual void AtDestroy() { }

}