using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobileEntity<M> : BaseEntity<M> where M: BaseModel
{
    public abstract float Hp { get; set; }

    public virtual void Damaged(float damage)
    {
        Hp -= damage;
    }
}
