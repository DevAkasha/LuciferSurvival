using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour { } // 모든 컨트롤러의 기본 클래스 (MonoBehaviour 상속)
public abstract class BaseController<E, M> : Controller where E : BaseEntity<M> where M : BaseModel // Entity와 Model을 연결하는 제네릭 컨트롤러
{
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
public abstract class BaseController<M> : Controller where M : BaseModel
{

}

public abstract class BaseController : Controller
{ 

}
