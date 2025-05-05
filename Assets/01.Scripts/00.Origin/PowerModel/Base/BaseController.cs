using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


public abstract class Controller : MonoBehaviour // 모든 컨트롤러의 기본 클래스 (MonoBehaviour 상속)
{
    protected bool isStartInit;

    protected virtual void AtDestroy() { }
    protected virtual void AtDisable() { }
    protected virtual void AtInit() { }
} 
public abstract class MController : Controller, IModelOwner
{
    public abstract BaseModel GetBaseModel();
}

public abstract class BaseController<E, M> : MController, IRxCaller where E : BaseEntity<M> where M : BaseModel // Entity와 Model을 연결하는 제네릭 컨트롤러
{
    
    bool IRxCaller.IsLogicalCaller => true;
    bool IRxCaller.IsMultiRolesCaller => false;
    bool IRxCaller.IsFunctionalCaller => false;
 
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

    protected virtual void OnDisable()
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
        AtDestroy();
    }

}

public abstract class BaseController<M> : MController, IRxCaller, IModelOwner<M> where M : BaseModel
{
    public M Model { get; set; }

    public bool IsLogicalCaller => true;

    public bool IsMultiRolesCaller => true;

    public bool IsFunctionalCaller => false;

    public override BaseModel GetBaseModel() => Model;

    public M GetModel() => Model;

    protected virtual void Start()
    {
        AtInit();
        isStartInit = true;
    }

    protected virtual void OnEnable()
    {
        if (isStartInit) return;
        AtInit();
    }

    protected virtual void OnDisable()
    {
        Model?.Unload();
        isStartInit = false;
        AtDisable();
    }
    protected virtual void OnDestroy()
    {
        Model?.Unload();
        AtDestroy();
    }

    protected abstract void SetupModel();

}

public abstract class BaseController : Controller, IRxCaller, IRxOwner
{
    public bool IsLogicalCaller => true;

    public bool IsMultiRolesCaller => true;

    public bool IsFunctionalCaller => false;

    public bool IsRxVarOwner => true;

    public bool IsRxAllOwner => false;

    private readonly HashSet<RxBase> trackedRxVars = new();


    public void RegisterRx(RxBase rx) // Rx 필드를 모델에 등록
    {
        trackedRxVars.Add(rx);
    }

    public void Unload()
    {
        foreach (var rx in trackedRxVars)
        {
            rx.ClearRelation();
        }
        trackedRxVars.Clear();
    }

    protected virtual void OnEnable() => AtInit();
    protected virtual void OnDisable() => AtDisable();
    protected virtual void OnDestroy() => AtDestroy();
}



