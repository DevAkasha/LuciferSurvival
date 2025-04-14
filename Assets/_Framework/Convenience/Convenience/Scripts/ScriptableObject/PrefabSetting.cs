using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabSetting", menuName = "ScriptableObjects/PrefabSetting")]
public class PrefabSetting : SOSingleton<PrefabSetting>
{
    public Object prefabPath;
#if UNITY_EDITOR
    public static string path { get => AssetDatabase.GetAssetPath(instance.prefabPath); }
#endif
}