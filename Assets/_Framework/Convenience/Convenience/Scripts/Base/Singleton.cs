using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ironcow.Convenience
{
    public class Singleton<T>
    {
        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)Activator.CreateInstance(typeof(T));
                }
                return _instance;
            }
        }

        public static bool isInstance { get => _instance != null; }

    }
}