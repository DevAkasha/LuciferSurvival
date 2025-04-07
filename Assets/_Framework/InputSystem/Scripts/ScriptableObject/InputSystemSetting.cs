using System.IO;
using UnityEditor;
using UnityEngine;

    [InitializeOnLoad]
public class InputSystemSetting : SOSingleton<InputSystemSetting>
{
    [Header("Create Key Bind Path")]
    public Object createKeyBindPath;
    public static string CreateKeyBindPath { get => $"{AssetDatabase.GetAssetPath(Instance.createKeyBindPath)}/KeyBindDatas/"; }

}
