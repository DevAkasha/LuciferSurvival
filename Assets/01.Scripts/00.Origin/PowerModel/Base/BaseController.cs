using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour { } // 모든 컨트롤러의 기본 클래스 (MonoBehaviour 상속)
public abstract class EMController : Controller { } // 모든 컨트롤러의 기본 클래스 (MonoBehaviour 상속)

public abstract class BaseController<E, M> : EMController, IRxCaller where E : BaseEntity<M> where M : BaseModel // Entity와 Model을 연결하는 제네릭 컨트롤러
{
    bool IRxCaller.IsLogicalCaller => true;
    bool IRxCaller.IsMultiRolesCaller => false;
    bool IRxCaller.IsFunctionalCaller => false;
 
    [SerializeField] private E entity;

    public E Entity => entity;

    protected virtual void OnEnable()
    {
        if (entity == null) entity = GetComponent<E>();
        if (entity != null) OnEntityInjected(); // Entity가 주입된 이후의 초기화 처리
        OnInit();
    }

    public void InjectEntity(E entity) // 외부에서 Entity를 주입
    {
        this.entity = entity;
        OnEntityInjected(); // Entity가 주입된 이후의 초기화 처리
    }

    protected virtual void OnInit() { }
    protected virtual void OnEntityInjected() { } // Entity가 주입된 이후의 초기화 처리
}
public abstract class MController : Controller, IModelOwner
{
    public abstract BaseModel GetBaseModel();
}

public abstract class BaseController<M> : MController, IRxCaller, IModelOwner<M> where M : BaseModel
{
    bool IRxCaller.IsLogicalCaller => true;
    bool IRxCaller.IsMultiRolesCaller => true;
    bool IRxCaller.IsFunctionalCaller => false;

    public M Model { get; set; }

    public override BaseModel GetBaseModel() => Model;
    public M GetModel() => Model;

    protected virtual void OnDisable() => Model?.Unload();
    protected virtual void OnDestroy() => Model?.Unload();
}

public abstract class BaseController : Controller, IRxCaller, IRxOwner
{
    bool IRxOwner.IsRxVarOwner => true;
    bool IRxOwner.IsRxAllOwner => false;

    bool IRxCaller.IsLogicalCaller => true;
    bool IRxCaller.IsMultiRolesCaller => true;
    bool IRxCaller.IsFunctionalCaller => false;

    private readonly HashSet<RxBase> trackedRxVars = new();

    public void RegisterRx(RxBase rx)
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
}
