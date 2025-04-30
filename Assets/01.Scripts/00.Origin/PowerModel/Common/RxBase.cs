using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RxBase : IConditionCheckable
{
    public abstract void ClearRelation();
    public virtual bool Satisfies(Func<object, bool> predicate) => false;
}
