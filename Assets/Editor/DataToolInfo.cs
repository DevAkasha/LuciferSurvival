using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

//[CustomEditor(typeof(DataManager))]
public class DataToolInfo : Editor
{
    //private List<SheetInfo> sheets { get; }
    public BaseDataSO currentAsset = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("DataSO 생성"))
        {
            DrawScriptableObject();
        }
    }
    public void DrawScriptableObject()
    {
        var sheetsField = ((GSpreadReader<DataManager>)target).Sheets;
        var sheets = sheetsField.FindAll(obj => obj.className == currentAsset.GetType().ToString());
        
        DownloadData(sheets);

        //currentAsset = EditorGUILayout.ObjectField("", currentAsset, currentAsset.GetType().DeclaringType, true) as BaseDataSO;
        //var editor = Editor.CreateEditor(currentAsset);
        //editor.OnInspectorGUI();
    }
    public async void DownloadData(List<SheetInfo> sheets, BaseDataSO data = null)
    {
        foreach (var sheet in sheets)
        {
            var url = $"{DataToolSetting.Instance.GSheetUrl}export?format=tsv&gid={sheet.sheetId}";
            var req = UnityWebRequest.Get(url);
            var op = req.SendWebRequest();
            Debug.Log($"{sheet.className}");
            await op;
            var res = req.downloadHandler.text;
            Debug.Log(res);
            sheet.datas = TSVParser.TsvToDic(res);
        }
        if (data != null)
        {
            RefreshData(sheets[0], data);
        }
        else
        {
            ImportDatas(sheets);
        }
    }

    public void RefreshData(SheetInfo sheet, BaseDataSO data)
    {
        var dicData = sheet.datas.Find(obj => obj["rcode"] == data.rcode);
        var path = DataToolSetting.DataScriptableObjectPath + "/" + dicData["rcode"] + ".asset";
        var dt = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, data.GetType());
        dt = TSVParser.DicToSOData(data.GetType(), dt, dicData);

        EditorUtility.SetDirty(dt);
        AssetDatabase.SaveAssets();
    }

    protected void ImportDatas(List<SheetInfo> sheets)
    {
        foreach (var sheet in sheets)
        {
            ImportData(sheet);
        }
    }

    protected void ImportData(SheetInfo sheet)
    {
        Assembly assembly = typeof(BaseDataSO).Assembly;
        var type = assembly.GetType(sheet.className);
        GetDatas(type, sheet.datas);
    }

    public void GetDatas(Type type, List<Dictionary<string, string>> datas)
    {
        foreach (var data in datas)
        {
            var path = DataToolSetting.DataScriptableObjectPath + "/" + data["rcode"] + ".asset";
            var dt = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, type);
            if (dt == null)
            {
                dt = DicToClass(type, data);
            }
            else
            {

                dt = TSVParser.DicToSOData(type, dt, data);
            }

            EditorUtility.SetDirty(dt);
            AssetDatabase.SaveAssets();
        }
    }

    public ScriptableObject DicToClass(Type type, Dictionary<string, string> data)
    {
        var dt = CreateInstance(type);
        AssetDatabase.CreateAsset(dt, DataToolSetting.DataScriptableObjectPath + "/" + data["rcode"] + ".asset");
        return TSVParser.DicToSOData(type, dt, data);
    }
}
