using Ironcow.Convenience;
using Ironcow.ObjectPool;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
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
                if (instance == null)
                {
#if UNITY_EDITOR
                    T instance = CreateInstance<T>();
                    string directory = Application.dataPath.Replace("Assets", EditorDataSetting.SettingSOPath);
                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                        AssetDatabase.Refresh();
                    }
                    string assetPath = $"{EditorDataSetting.SettingSOPath}/{name}.asset";
                    AssetDatabase.CreateAsset(instance, assetPath);
#endif
                }
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
