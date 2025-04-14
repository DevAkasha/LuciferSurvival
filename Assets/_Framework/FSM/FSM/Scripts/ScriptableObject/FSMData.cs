using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class FSMData : SOSingleton<FSMData>
{
#if UNITY_EDITOR
    [MenuItem("Ironcow/Data/FSMData Settings")]
    private static void Edit()
    {
        Selection.activeObject = instance;
    }
#endif

    public string fsmName;
    public List<string> states;
#if UNITY_EDITOR
    public Object templeteFolder;
    public static string TempleteFolder { get => AssetDatabase.GetAssetPath(instance.templeteFolder); }
    public Object outPath;
    public static string OutPath { get => AssetDatabase.GetAssetPath(instance.outPath); }
#endif
}