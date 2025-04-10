using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModel : BaseModel
{
    public RxVar<float> Hp = new RxVar<float>(100);

    public override IEnumerable<IModifiable> GetModifiables()
    {
        throw new System.NotImplementedException();
    }
}
