using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseEntity : WorldObject { }

public interface IBaseEntity<M> where M : BaseModel
{
   public M Model { get; set; }
}

public abstract class BaseEntity<M> : BaseEntity, IBaseEntity<M> where M : BaseModel
{
    public M Model { get ; set; }

    public virtual M GetModel()
    {
        if (Model == null)
            SetupModels();
        return Model;
    }

}
