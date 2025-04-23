using System;
using System.Collections.Generic;

public interface ITrackableRxModel
{
    void RegisterRx(RxBase rx); // Rx 필드를 모델에 등록
}

public interface IModifiableTarget
{
    IEnumerable<IModifiable> GetModifiables(); // 수정 가능한 필드 목록 반환
}

public abstract class BaseModel : IModifiableTarget, ITrackableRxModel
{
    private readonly List<RxBase> trackedRxVars = new();
    private readonly List<IModifiable> modifiables = new();

    public void RegisterRx(RxBase rx) // Rx 필드를 모델에 등록
    {
        if (!trackedRxVars.Contains(rx))
            trackedRxVars.Add(rx);

        if (rx is IModifiable mod)
            RegisterModifiable(mod);
    }

    protected void RegisterModifiable(IModifiable modifiable)
    {
        if (modifiable != null && !modifiables.Contains(modifiable))
            modifiables.Add(modifiable);
    }

    public virtual IEnumerable<IModifiable> GetModifiables() => modifiables; // 수정 가능한 필드 목록 반환
    
    public IEnumerable<RxBase> GetAllRxFields() => trackedRxVars;

    public void Unload()
    {
        foreach (var rx in trackedRxVars)
        {
            rx.ClearRelation();
        }
        trackedRxVars.Clear();
        modifiables.Clear();
    }
}