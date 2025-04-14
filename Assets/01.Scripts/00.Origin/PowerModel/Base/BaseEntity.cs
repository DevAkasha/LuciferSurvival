using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseEntity : WorldObject { }

public interface IBaseEntity<M> where M : BaseModel
{
   public M Model { get; set; }
   public M GetModel();
}

public abstract class BaseEntity<M> : BaseEntity, IBaseEntity<M> where M : BaseModel
{
    public M Model { get ; set; }

    protected virtual void OnDisable()
    {
        Model.Unload();
    }
    protected virtual void OnDestroy()
    {
        Model.Unload();
    }
    public virtual M GetModel()
    {
        return Model;
    }
}
