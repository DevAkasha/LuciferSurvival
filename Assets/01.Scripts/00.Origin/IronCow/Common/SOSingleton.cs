using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class SOSingleton<T> : ScriptableObject where T : ScriptableObject
{
    static private T instance = null;
    static public T Instance
    {
        get
        {
            if (instance == null)
            {
                var name = typeof(T).Name;
                instance = Resources.Load<T>(name);
            }

            return instance;
        }
    }

#if UNITY_EDITOR
    private static void Edit()
    {
        Selection.activeObject = Instance;
    }

    public void SaveData()
    {
        EditorUtility.SetDirty(instance);
    }
#endif
}
