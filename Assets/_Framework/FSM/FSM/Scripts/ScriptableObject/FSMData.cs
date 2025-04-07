using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FSMData : SOSingleton<FSMData>
{
    [MenuItem("Ironcow/Data/FSMData Settings")]
    private static void Edit()
    {
        Selection.activeObject = Instance;
    }

    public string fsmName;
    public List<string> states;
    public Object templeteFolder;
    public static string TempleteFolder { get => AssetDatabase.GetAssetPath(Instance.templeteFolder); }
    public Object outPath;
    public static string OutPath { get => AssetDatabase.GetAssetPath(Instance.outPath); }
}