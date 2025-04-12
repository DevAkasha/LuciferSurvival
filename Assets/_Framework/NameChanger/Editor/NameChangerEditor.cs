using Ironcow;
using Ironcow.Data;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(NameChanger))]
public class NameChangerEditor : Editor
{
    NameChanger instance => (NameChanger)this.target;
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("형식 맞추기"))
        {
            var files = Directory.GetFiles(Application.dataPath.Replace("Assets", NameChanger.PrefabPath)).ToList();
            files.RemoveAll(obj => obj.Contains(".json"));
            files.RemoveAll(obj => obj.Contains(".meta"));
            foreach (var file in files)
            {
                var path = file.Replace(Application.dataPath, "Assets");
                path = path.Replace("\\", "/");
                var newPath = path;
                if (path.Contains("ProjectileSub"))
                {
                    newPath = path.Replace("ProjectileSub", "Projectile_Sub");
                }
                if (path.Contains("Projectile1"))
                {
                    newPath = newPath.Replace("Projectile1", "Projectile_Sub");
                }
                if (path.Contains("Projectile2"))
                {
                    newPath = newPath.Replace("Projectile2", "Projectile_Sub2");
                }
                if (path.Split('_').Length == 2)
                {
                    newPath = newPath.Replace("_", "_idle_");
                }
                if (newPath != path)
                {
                    AssetDatabase.RenameAsset(path, newPath.Split('/').Last());
                }
            }
        }
        if (GUILayout.Button("이름 변경"))
        {
            var files = Directory.GetFiles(Application.dataPath.Replace("Assets", NameChanger.PrefabPath)).ToList();
            files.RemoveAll(obj => obj.Contains(".json"));
            files.RemoveAll(obj => obj.Contains(".meta"));
            foreach (var dt in instance.datas)
            {
                var data = dt as BaseDataSO;
                var key = data.rcode;
                var oldName = "";// data.effectName;
                var selectedList = files.FindAll(obj => obj.Contains(oldName));
                foreach(var file in selectedList)
                {
                    var path = file.Replace(Application.dataPath, "Assets");
                    path = path.Replace("\\", "/");
                    if (!path.FileName().Split('_')[0].Equals(oldName))
                    {
                        continue;
                    }
                    var middleKey = "idle";
                    if (path.Contains("Sub4"))
                    {
                        middleKey = "sub4";
                    }
                    else if (path.Contains("Sub3"))
                    {
                        middleKey = "sub3";
                    }
                    else if (path.Contains("Sub2"))
                    {
                        middleKey = "sub2";
                    }
                    else if (path.Contains("Sub"))
                    {
                        middleKey = "sub";
                    }
                    var lastKey = path.Split('_').Last();
                    var newName = $"{key}_{middleKey}_{lastKey}";
                    AssetDatabase.RenameAsset(path, newName);
                }
            }
        }
        GUILayout.EndHorizontal();
        SetList("Change State", typeof(ChangeForm));
        base.OnInspectorGUI();
    }

    public void SetList(string label, Type type)
    {
        var list = instance.changes;
        ReorderableList rlist = new ReorderableList(list, type, true, true, true, true);
        rlist.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, label.ToUpper());
        };
        rlist.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            rect.height -= 4;
            rect.y += 2;
            EditorGUI.LabelField(new Rect(rect.position.x, rect.position.y, 60, rect.height), "From");
            list[index].from = EditorGUI.TextField(new Rect(rect.position.x + 60, rect.position.y, rect.width / 2 - 70, rect.height), list[index].from);
            EditorGUI.LabelField(new Rect(rect.position.x + rect.width / 2, rect.position.y, 60, rect.height), "To");
            list[index].to = EditorGUI.TextField(new Rect(rect.position.x + rect.width / 2 + 60, rect.position.y, rect.width / 2 - 70, rect.height), list[index].to);
        };
        rlist.onAddCallback = lt =>
        {
            instance.changes.Add(new ChangeForm());
        };
        rlist.DoLayoutList();
    }
}
