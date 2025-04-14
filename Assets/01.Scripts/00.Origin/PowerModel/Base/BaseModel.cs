using System;
using System.Collections.Generic;

public interface ITrackableRxModel
{
    void RegisterRx(RxBase rx);
}

public interface IModifiableTarget
{
    IEnumerable<IModifiable> GetModifiables();
}

public abstract class BaseModel : IModifiableTarget, ITrackableRxModel
{
    private readonly List<RxBase> trackedRxVars = new();
    private readonly List<IModifiable> modifiables = new();

    public void RegisterRx(RxBase rx)
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

    public virtual IEnumerable<IModifiable> GetModifiables() => modifiables;

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