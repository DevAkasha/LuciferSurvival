using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobileEntity<M> : BaseEntity<M> where M: BaseModel
{
    public abstract float Health { get; set; }
    
    public virtual void TakeDamaged(float damage)
    {
        Health -= damage;
    }
}
