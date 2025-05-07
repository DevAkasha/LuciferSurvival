using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Controller : MonoBehaviour // 모든 컨트롤러의 기본 클래스 (MonoBehaviour 상속)
{
    protected bool isInitialized;
    [SerializeField] protected bool isEnableLifecycle = true;

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
        Initialize();
    }

    protected virtual void OnEnable()
    {
        if (!isInitialized && isEnableLifecycle)
        {
            Initialize();
        }
    }

    protected virtual void OnDisable()
    {
        if (isEnableLifecycle)
        {
            Deinitialize();
        }
        entity.CallDisable();
        AtDisable();
    }
    protected virtual void OnDestroy() 
    {
        if (isEnableLifecycle)
        {
            Deinitialize();
        }
        entity.CallDestroy();
        AtDestroy();
    }

    private void Initialize()
    {
        if (entity == null)
            entity = GetComponentInChildren<E>();

        entity.CallInit();
        AtInit();
        isInitialized = true;
    }

    private void Deinitialize()
    {
        Model?.Unload(); 
        isInitialized = false;
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
        Initialize();
    }

    protected virtual void OnEnable()
    {
        if (!isInitialized && isEnableLifecycle)
            Initialize();
    }

    protected virtual void OnDisable()
    {
        if (isEnableLifecycle)
        {
            Model?.Unload();
            isInitialized = false;
        }
        AtDisable();
    }

    protected virtual void OnDestroy()
    {
        Model?.Unload();
        isInitialized = false;
        AtDestroy();
    }

    protected abstract void SetupModel();

    private void Initialize()
    {
        SetupModel();
        AtInit();
        isInitialized = true;
    }
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