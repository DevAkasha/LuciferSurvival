using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController<E> : BaseController<E> where E : BaseEntity
{
    [SerializeField]
    private BTRunner unitBTRunner;


}
