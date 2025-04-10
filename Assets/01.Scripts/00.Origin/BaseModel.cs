using System.Collections.Generic;

public abstract class BaseModel : IModifiableTarget, ITrackableRxModel
{
    private readonly List<RxBase> trackedRxVars = new();

    public void RegisterRx(RxBase rx)
    {
        if (!trackedRxVars.Contains(rx))
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

    public abstract IEnumerable<IModifiable> GetModifiables();
}

public interface ITrackableRxModel
{
    void RegisterRx(RxBase rx);
}

public abstract class RxBase
{
    public abstract void ClearRelation();
}
