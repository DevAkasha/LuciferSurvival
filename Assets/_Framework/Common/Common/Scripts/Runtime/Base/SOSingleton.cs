using Ironcow.Convenience;
using Ironcow.ObjectPool;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class SOSingleton<T> : ScriptableObject where T : ScriptableObject
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
                if (_instance == null)
                {
#if UNITY_EDITOR
                    _instance = AssetDatabase.LoadAssetAtPath<T>(Path.Combine(EditorDataSetting.SettingSOPath, name + ".asset"));
                    if (_instance == null)
                    {
                        T instance = CreateInstance<T>();
                        string directory = Application.dataPath.Replace("Assets", EditorDataSetting.SettingSOPath);
                        if (!System.IO.Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                            AssetDatabase.Refresh();
                        }
                        string assetPath = $"{EditorDataSetting.SettingSOPath}/{name}.asset";
                        AssetDatabase.CreateAsset(instance, assetPath);
                    }
#endif
                }
            }

            return _instance;
        }
    }

#if UNITY_EDITOR
    private static void Edit()
    {
        Selection.activeObject = instance;
    }

    public void SaveData()
    {
        EditorUtility.SetDirty(_instance);
    }
#endif
}
