using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public abstract class BaseController : MonoBehaviour { }
public abstract class BaseController<E> : BaseController where E : BaseEntity
{
    [SerializeField] private E entity;

    public E Entity => entity;

    protected virtual void Awake()
    {
        if (entity == null) entity = GetComponent<E>();
        if (entity != null) OnEntityInjected();
        OnInit();
    }

    public void InjectEntity(E entity)
    {
        this.entity = entity;
        OnEntityInjected();
    }

    protected virtual void OnInit() { }
    protected virtual void OnEntityInjected() { }
}


