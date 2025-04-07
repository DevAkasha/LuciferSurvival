using Ironcow.Data;
using Ironcow.WorldObjectBase;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ironcow.FSM
{
    public abstract class FSMController<S, D> : WorldBase<D> where S : BaseState where D : BaseDataSO
    {
        protected FSM<S> fsm;
        public Dictionary<string, S> states = new Dictionary<string, S>();
        public S currentState { get => fsm.currentState; }

        protected virtual bool ChangeState<T>(out S state) where T : S
        {
            if (states.ContainsKey(typeof(T).ToString()))
            {
                fsm.ChangeState((T)states[typeof(T).ToString()]);
                state = (T)states[typeof(T).ToString()];
                return false;
            }
            else
            {
                state = CreateState<T>();
                return true;
                //fsm.ChangeState(CreateState<T>());
            }
            //return (T)states[typeof(T).ToString()];
        }

        protected T CreateState<T>() where T : S
        {
            var state = Activator.CreateInstance(typeof(T));
            return AddState((T)state);
        }

        protected T AddState<T>(T state) where T : S
        {
            states.Add(typeof(T).ToString(), state);
            return (T)states[typeof(T).ToString()];
        }

        protected virtual bool ChangeState(Type type, out S state)
        {
            if (states.ContainsKey(type.ToString()))
            {
                fsm.ChangeState(states[type.ToString()]);
                state = states[type.ToString()];
                return false;
            }
            else
            {
                state = CreateState(type);
                return true;
            }
            //return states[type.ToString()];
        }

        protected S CreateState(Type type)
        {
            var state = Activator.CreateInstance(type);
            return AddState(type, (S)state);
        }

        protected S AddState(Type type, S state)
        {
            states.Add(type.ToString(), state);
            return states[type.ToString()];
        }
    }

}