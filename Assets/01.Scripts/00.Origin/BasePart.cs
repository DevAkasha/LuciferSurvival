using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePart : WorldObject { }

public abstract class BasePart<E,M> : BasePart where E: IBaseEntity<M> where M : BaseModel
{
    protected E Entity { get; set; }
    protected M Model { get; set; }

    private void Start()
    {
        Entity = (E)GetComponent<IBaseEntity<M>>();
        SetupModels();
    }

    protected override void SetupModels()
    {
        Model = Entity.Model;
    }
}
