using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BTEditor", menuName = "ScriptableObjects/BTEditor")]
public class BTEditor : SOSingleton<BTEditor>
{
	public Object savePath;
	public static string ParentPath { get => AssetDatabase.GetAssetPath(Instance.savePath); }
	public static string SavePath { get => Path.Combine(AssetDatabase.GetAssetPath(Instance.savePath), "BTSaveData/"); }
}