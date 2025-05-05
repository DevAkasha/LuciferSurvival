using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour { } // 모든 컨트롤러의 기본 클래스 (MonoBehaviour 상속)
<<<<<<< Updated upstream
public abstract class BaseController<E, M> : Controller where E : BaseEntity<M> where M : BaseModel // Entity와 Model을 연결하는 제네릭 컨트롤러
{
=======

public abstract class EMController : Controller , IModelOwner
{
    public abstract BaseModel GetBaseModel();
}

public abstract class BaseController<E, M> : EMController, IRxCaller where E : BaseEntity<M> where M : BaseModel // Entity와 Model을 연결하는 제네릭 컨트롤러
{
    private bool isStartInit;
    bool IRxCaller.IsLogicalCaller => true;
    bool IRxCaller.IsMultiRolesCaller => false;
    bool IRxCaller.IsFunctionalCaller => false;
 
>>>>>>> Stashed changes
    [SerializeField] private E entity;

    public E Entity => entity;

    public M Model => entity.Model;

    public override BaseModel GetBaseModel() => Model;

    public M GetModel() => Model;



    protected virtual void Start()
    {
        if (entity == null) 
            entity = GetComponent<E>();

        if (entity != null)    
            entity.CallInit();
        
        AtInit();
        isStartInit = true;
    }

    protected virtual void OnEnable()
    {
        if (isStartInit) return;

        if (entity == null) 
            entity = GetComponent<E>();

        if (entity != null)
            entity.CallInit();
       
        AtInit();
    }

    private void OnDisable()
    {
        Model?.Unload();
        entity.AtDisable();
        isStartInit = false;
        AtDisable();
    }
    protected virtual void OnDestroy() 
    {
        Model?.Unload();
        entity.AtDestroy();
    }

    protected virtual void AtDisable() { }
    protected virtual void AtInit() { }

}
<<<<<<< Updated upstream
public abstract class BaseController<M> : Controller where M : BaseModel
=======

public abstract class MController : Controller, IModelOwner
>>>>>>> Stashed changes
{

}

public abstract class BaseController : Controller
{ 

}
