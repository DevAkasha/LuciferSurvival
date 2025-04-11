using Ironcow.FSM;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(FSMData))]
public class FSMDataDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        var target = this.target as FSMData;
        target.templeteFolder = EditorGUILayout.ObjectField("Templete Path", target.templeteFolder, typeof(Object));
        GUILayout.Space(20);
        target.fsmName = EditorGUILayout.TextField("State Name", target.fsmName);

        GUILayout.Space(20);
        ReorderableList list = new ReorderableList(target.states, typeof(string), true, true, true, true);
        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "States");
        };
        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            rect.height -= 4;
            rect.y += 2;
            if(target.states.Count < index)
            {
                target.states.Add("");
            }
            target.states[index] = EditorGUI.TextField(rect, target.states[index]);
        };
        list.DoLayoutList();

        target.outPath = EditorGUILayout.ObjectField("Out Path", target.outPath, typeof(Object));

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Clear Datas"))
        {
            target.fsmName = "";
            target.states.Clear();
        }
        if(GUILayout.Button("Create States"))
        {
            var path = $"{FSMData.OutPath}/{target.fsmName}FSM";
            if(AssetDatabase.LoadAssetAtPath(path, typeof(Object)))
            {
                path = AssetDatabase.CreateFolder(FSMData.OutPath, $"{target.fsmName}FSM");
            }
            CreateBaseState(path);
            for (int i = 0; i < target.states.Count; i++)
            {
                CreateState(path, target.states[i]);
            }
        }
        AssetDatabase.Refresh();
        GUILayout.EndHorizontal();
        EditorUtility.SetDirty(target);
    }

    public void CreateBaseState(string path)
    {
        var newPath = Path.Combine(path, $"{FSMData.instance.fsmName}BaseState.cs");
        var data = File.ReadAllText(Path.Combine(FSMData.TempleteFolder, "BaseStateTemplate.cs.txt").Replace("Assets", Application.dataPath));
        data = data.Replace("#SCRIPTNAME#", FSMData.instance.fsmName);
        File.WriteAllText(newPath, data);
    }

    public void CreateState(string path, string state)
    {
        var newPath = Path.Combine(path, $"{FSMData.instance.fsmName}{state}State.cs");
        var data = File.ReadAllText(Path.Combine(FSMData.TempleteFolder, "StateTemplate.cs.txt").Replace("Assets", Application.dataPath));
        data = data.Replace("#SCRIPTNAME#", $"{FSMData.instance.fsmName}{state}").Replace("#PARENT#", $"{FSMData.instance.fsmName}BaseState");
        File.WriteAllText(newPath, data);
    }
}