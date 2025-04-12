using Ironcow;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class KeyBindListData : SOSingleton<KeyBindListData>
{
    public List<string> keys = new List<string>();
    public List<KeyBindData> datas = new List<KeyBindData>();

#if UNITY_EDITOR
    public void Refresh()
    {
        this.datas.Clear();
        List<string> datas = new List<string>();
        datas = Directory.GetFiles(Application.dataPath.Replace("Assets", InputSystemSetting.CreateKeyBindPath)).ToList();
        datas.RemoveAll(obj => obj.Contains(".meta"));
        datas.RemoveAll(obj => obj.Contains(".json"));

        foreach (var path in datas)
        {
            var newPath = path.Replace(Application.dataPath, "Assets");
            this.datas.Add(AssetDatabase.LoadAssetAtPath<KeyBindData>(newPath));
        }
        EditorUtility.SetDirty(this);
    }
#endif
}
