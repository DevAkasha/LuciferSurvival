
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Ironcow
{
    [Serializable]
    public class ReactiveProperty<T> where T : struct
    {
        [SerializeField] T value;
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                CheckActions();
                action.Invoke(this.value);
            }
        }
        Action<T> action;
        Action<T> lastAction;
        Dictionary<GameObject, Action<T>> actions = new Dictionary<GameObject, Action<T>>();

        public bool IsSubscribe(GameObject go)
        {
            return actions.ContainsKey(go);
        }

        public ReactiveProperty<T> Subscribe(Action<T> action)
        {
            this.action += action;
            this.lastAction = action;
            this.action.Invoke(this.value);
            return this;
        }

        public ReactiveProperty<T> Unsubscribe(Action<T> action)
        {
            this.action -= action;
            return this;
        }

        public ReactiveProperty<T> AddTo(GameObject gameObject)
        {
            actions.Add(gameObject, lastAction);
            return this;
        }

        public void CheckActions()
        {
            var keys = new List<GameObject>(actions.Keys);
            if (!keys.Exists(obj => obj == null)) return;
            var values = new List<Action<T>>(actions.Values);
            actions.Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] == null)
                {
                    Unsubscribe(values[i]);
                }
                else
                {
                    actions.Add(keys[i], values[i]);
                }
            }
        }
    }
}