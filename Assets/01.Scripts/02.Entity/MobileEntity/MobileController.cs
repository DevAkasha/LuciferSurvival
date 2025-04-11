using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobileController<E,M> : BaseController<E,M> where E : MobileEntity<M> where M: BaseModel
{

}