using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class SOSingletonLite<T> : ScriptableObject where T : ScriptableObject
{
    static private T _instance = null;
    static public T instance
    {
        get
        {
            if (_instance == null)
            {
                var name = typeof(T).Name;
                _instance = Resources.Load<T>(name);

            }

            return _instance;
        }
    }

#if UNITY_EDITOR
    public void SaveData()
    {
        EditorUtility.SetDirty(_instance);
    }
#endif
}
