using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel: BaseModel
{
   public RxVar<float> Hp = new(100);
   public RxVar<float> MoveSpeed = new(4);
}
