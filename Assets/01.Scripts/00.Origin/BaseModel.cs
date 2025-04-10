
using System.Collections.Generic;

public abstract class BaseModel : IModifiableTarget
{
    public abstract IEnumerable<IModifiable> GetModifiables();
}
