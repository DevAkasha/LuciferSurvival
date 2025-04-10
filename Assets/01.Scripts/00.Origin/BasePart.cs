using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePart : WorldObject { }

public abstract class BasePart<E,M> : BasePart where E: IBaseEntity<M> where M : BaseModel
{
    protected E Entity { get; set; }
    protected M Model { get; set; }
    protected virtual void Start()
    {
        Model ??= Entity.GetModel();
        OnStart();
    }
    protected override void SetupModels()
    {
        Entity = (E)GetComponent<IBaseEntity<M>>();
        Model = Entity.GetModel();
    }

    protected virtual void OnStart() { }
}
