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
    private BasePart[] partList;
    public M Model { get; set; }

    public override BaseModel GetBaseModel() => Model;

    public M GetModel() => Model;
    
    public void CallInit()
    {
        SetupModel();
        AtInit();

        partList = GetComponentsInChildren<BasePart>();
        foreach (BasePart part in partList)
        {
            part.RegistEntity(this);
            part.RegistModel(Model);
            part.CallInit();
        }
    }

    public void CallDisable()
    {
        foreach (BasePart part in partList)
        {
            part.CallDisable();
        }
        AtDisable();
    }

    public void CallDestroy()
    {
        foreach (BasePart part in partList)
        {
            part.CallDestroy();
        }
        AtDestroy();
    }

    protected abstract void SetupModel();
    protected virtual void AtInit() { }
    public virtual void AtDisable() { }
    public virtual void AtDestroy() { }

}