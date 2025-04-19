using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class InputSystemSetting : SOSingleton<InputSystemSetting>
{
    [Header("Create Key Bind Path")]
    public Object createKeyBindPath;
#if UNITY_EDITOR
    public static string CreateKeyBindPath { get => $"{AssetDatabase.GetAssetPath(Instance.createKeyBindPath)}/"; }
#endif

}
