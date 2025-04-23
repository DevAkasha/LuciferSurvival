using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class DataTool : EditorWindow
{
    public static DataTool instance;
    [MenuItem("Tools/Data Tool #&x")]
    public static void Open()
    {
        if (instance == null)
        {
            var window = GetWindow<DataTool>();
            window.minSize = new Vector2(512f, 728f);
            instance = window;
        }
    }

    private Vector2 sceneViewScrollPosition = Vector2.zero;
    private Vector2 windowScrollPosition = Vector2.zero;

    private void OnFocus()
    {
        if (instance == null)
            this.OnEnable();
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void OnEnable()
    {
        try
        {
            if (instance == null)
            {
                var window = GetWindow<DataTool>();
                window.minSize = new Vector2(512f, 728f);
                instance = window;
            }

            datas = Directory.GetFiles(DataToolSetting.DataScriptableObjectFullPath).ToList();
            datas.RemoveAll(obj => obj.Contains(".meta"));
            datas.RemoveAll(obj => obj.Contains(".json"));

            isOpened.Clear();
            foreach (var sheet in sheets)
            {
                isOpened.Add(sheet.className, false);
            }
        }
        catch (Exception ex)
        {
            instance = null;
        }
    }

    public void InitThumbnail()
    {
        for (int i = 0; i < datas.Count; i++)
        {
            var asset = AssetDatabase.LoadAssetAtPath<BaseDataSO>(datas[i].Replace(Application.dataPath, "Assets"));
            if (asset == null) continue;
            var field = asset.GetType().GetField("thumbnail");
            if (field != null)
            {
                var rcode = asset.rcode;
                var path = DataToolSetting.ThumbnailPath + "/" + rcode + ".png";
                var texture = (Sprite)field.GetValue(asset);
                if (asset.GetType().GetField("createItem") != null)
                    rcode = (string)asset.GetType().GetField("createItem").GetValue(asset);
                if (asset.GetType().GetField("target") != null)
                    rcode = (string)asset.GetType().GetField("target").GetValue(asset);
                if (texture == null)
                {
                    field.SetValue(asset, AssetDatabase.LoadAssetAtPath(path, field.FieldType) as Sprite);
                    asset.SetDirty();
                }
            }
        }
        AssetDatabase.SaveAssets();
    }

    List<string> datas = new List<string>();
    string selectRCode;
    public BaseDataSO currentAsset = null;
    Dictionary<string, bool> isOpened = new Dictionary<string, bool>();
    private void Draw()
    {
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        EditorGUI.indentLevel++;
        {
            EditorGUI.indentLevel++;
            this.sceneViewScrollPosition = EditorGUILayout.BeginScrollView(this.sceneViewScrollPosition, "box", GUILayout.Width(220));
            {
                int index = 0;
                string lastID = "";

                EditorGUI.indentLevel -= 2;
                foreach (var sheet in sheets)
                {
                    var key = sheet.className;
                    bool opened = isOpened[key];
                    var style = new GUIStyle(UnityEngine.GUI.skin.button);
                    style.alignment = TextAnchor.MiddleLeft;
                    //opened = GUILayout.Toggle(opened, new GUIContent((opened ? "▼ " : "▶ ") + sheet.className), style, GUILayout.Width(180));
                    opened = EditorGUILayout.Foldout(opened, sheet.className);
                    isOpened[key] = opened;
                    lastID = key;

                    if (isOpened[key])
                    {
                        Dictionary<string, string> names = new Dictionary<string, string>();
                        var list = datas.FindAll(obj => obj.Contains(key));
                        foreach (var data in list)
                        {
                            var name = data.Split('\\').Last().Split('.')[0];
                            var path = DataToolSetting.DataScriptableObjectPath + "/" + name + ".asset";
                            names.Add(name, $"{name}");
                            index++;
                        }
                        var keys = new List<string>(names.Keys);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(10);
                        var toggleStyle = new GUIStyle(UnityEngine.GUI.skin.button);
                        toggleStyle.alignment = TextAnchor.MiddleRight;
                        var idx = GUILayout.SelectionGrid(keys.IndexOf(selectRCode), names.Values.ToArray(), 1, toggleStyle, GUILayout.Width(185));
                        EditorGUILayout.EndHorizontal();
                        if (idx >= 0)
                            selectRCode = keys[idx];
                    }
                }
                EditorGUI.indentLevel += 2;
            }
            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.Space(10);
        if (datas != null && datas.Count > 0)
        {
            currentAsset = AssetDatabase.LoadAssetAtPath<BaseDataSO>(DataToolSetting.DataScriptableObjectPath + "/" + selectRCode + ".asset");
            if (currentAsset != null)
            {
                GUILayout.BeginVertical();
                {
                    DrawScriptableObject();
                }
                GUILayout.EndVertical();
            }
        }
        GUILayout.EndHorizontal();
    }

    public void DrawScriptableObject()
    {

        //var sheetsField = ((GSpreadReader<DataManager>sa)target).Sheets;
        //var sheets = sheetsField.FindAll(obj => obj.className == currentAsset.GetType().ToString());

        DownloadData(sheets);
        //if (GUILayout.Button($"Refresh All {currentAsset.GetType().ToString()}s"))
        //{
        //    var sheets = this.sheets.FindAll(obj => obj.className == currentAsset.GetType().ToString());
        //    DownloadData(sheets);
        //}
        //if (GUILayout.Button($"Refresh {currentAsset.rcode}"))
        //{
        //    var sheets = this.sheets.FindAll(obj => obj.className == currentAsset.GetType().ToString());
        //    DownloadData(sheets, currentAsset);
        //}

        //currentAsset = EditorGUILayout.ObjectField("", currentAsset, currentAsset.GetType().DeclaringType, true) as BaseDataSO;
        //var editor = Editor.CreateEditor(currentAsset);
        //editor.OnInspectorGUI();
    }

    private void OnGUI()
    {
        DrawGUI();
    }

    public static void DrawGUIInspector()
    {
        instance = new DataTool();
        instance.DrawGUI();
    }

    public void DrawGUI()
    {
        this.windowScrollPosition = EditorGUILayout.BeginScrollView(this.windowScrollPosition, "box");
        {
            this.Draw();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Init Thumbnail")))
        {
            InitThumbnail();
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button(new GUIContent("Download Google Sheet")))
        {
            ClearLog();
            DownloadData(sheets);
        }
        GUILayout.EndHorizontal();
    }

    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
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

    private List<SheetInfo> sheets { get => DataToolSetting.Instance.sheets; }

    public ScriptableObject DicToClass(Type type, Dictionary<string, string> data)
    {
        var dt = CreateInstance(type);
        AssetDatabase.CreateAsset(dt, DataToolSetting.DataScriptableObjectPath + "/" + data["rcode"] + ".asset");
        return TSVParser.DicToSOData(type, dt, data);
    }
}
