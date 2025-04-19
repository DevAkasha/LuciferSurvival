using Ironcow.Data;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ChangeForm
{
    public string from;
    public string to;
}

[CreateAssetMenu(fileName = "NameChanger", menuName = "ScriptableObjects/NameChanger")]
public class NameChanger : SOSingleton<NameChanger>
{
    [SerializeReference] public List<BaseDataSO> datas;
    [SerializeField] public List<ChangeForm> changes = new List<ChangeForm>();

    [Header("Prefab Path")]
    public Object prefabPath;
#if UNITY_EDITOR
    public static string PrefabPath { get => AssetDatabase.GetAssetPath(Instance.prefabPath); }
#endif
}
