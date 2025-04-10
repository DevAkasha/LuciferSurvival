using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerModel: BaseModel
{
    public RxModFloat Hp = new(100);
    public RxModFloat MoveSpeed = new(4);

    public override IEnumerable<IModifiable> GetModifiables()
    {
        yield return Hp;
        yield return MoveSpeed;
    }
}