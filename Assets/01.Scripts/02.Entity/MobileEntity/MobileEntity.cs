using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobileEntity<M> : BaseEntity<M> where M: BaseModel
{
    protected abstract float CurHealth { get; set; }
    
    public virtual void TakeDamaged(float damage)
    {
        CurHealth -= damage;
    }
}
