using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ironcow.FSM
{
    public abstract class BaseState
    {
        public abstract void OnStateEnter();
        public abstract void OnStateUpdate();
        public abstract void OnStateExit();

        public void ChangeState<T>(FSM<T> fsm) where T : BaseState
        {
            fsm.ChangeState((T)this);
        }
    }
}