using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePart : WorldObject 
{
    public void CallInit() => AtInit();
    public void CallDisable() => AtDisable(); 
    public void CallDestroy() => AtDestroy();

    protected virtual void AtInit() { }
    protected virtual void AtDisable() { }
    protected virtual void AtDestroy() { }

}

public abstract class BasePart<E, M> : BasePart where E : BaseEntity where M : BaseModel
{
    protected E Entity { get; set; }
    protected M Model { get; set; }

    public void RegisterEntity(E entity)
    {
        Entity = entity;
    }

    public void RegisterModel(M model)
    {
        Model = model;
    }
}
