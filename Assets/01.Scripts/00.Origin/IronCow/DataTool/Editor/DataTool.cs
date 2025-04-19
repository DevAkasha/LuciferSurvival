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
using Ironcow.LocalizeTool;
using System.Linq;

public class DataTool : EditorWindow
{
    public static DataTool instance;
#if USE_SO_DATA
    [MenuItem("Ironcow/Tool/Data Tool #&x")]
#endif
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
                isOpened.Add(sheet.key, false);
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
#if USE_SO_DATA
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
#endif
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
                    var key = sheet.key;
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
                            names.Add(name,
#if USE_LOCALE
                                    $"{LocaleDataSO.GetString("name_" + name)}({name})"
#else
                                $"{name}"
#endif
                            );
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
#if USE_SO_DATA
            currentAsset = AssetDatabase.LoadAssetAtPath<BaseDataSO>(DataToolSetting.DataScriptableObjectPath + "/" + selectRCode + ".asset");
            if (currentAsset != null)
            {
                GUILayout.BeginVertical();
                {
                    DrawScriptableObject();
                }
                GUILayout.EndVertical();
            }
#endif
        }
        GUILayout.EndHorizontal();
    }

    public void DrawScriptableObject()
    {
        if (GUILayout.Button($"Refresh All {currentAsset.GetType().ToString()}s"))
        {
            var sheets = this.sheets.FindAll(obj => obj.className == currentAsset.GetType().ToString());
            DownloadData(sheets);
        }
        if (GUILayout.Button($"Refresh {currentAsset.rcode}"))
        {
            var sheets = this.sheets.FindAll(obj => obj.className == currentAsset.GetType().ToString());
            DownloadData(sheets, currentAsset);
        }
#if USE_SO_DATA
        currentAsset = EditorGUILayout.ObjectField("", currentAsset, currentAsset.GetType().DeclaringType, true) as BaseDataSO;
        var editor = Editor.CreateEditor(currentAsset);
        editor.OnInspectorGUI();
#endif
    }

    #region old
    public void DrawScriptableObjectToReflection()
    {

        EditorGUILayout.LabelField(LocaleDataSO.GetString("name_" + currentAsset.rcode));
        var fields = currentAsset.GetType().GetFields().ToList();
        var baseFields = typeof(BaseDataSO).GetFields().ToList();
        fields.InsertRange(0, fields.GetRange(fields.Count - baseFields.Count, baseFields.Count));
        fields.RemoveRange(fields.Count - baseFields.Count, baseFields.Count);
        fields.ForEach(fieldInfo =>
        {
            if (fieldInfo.FieldType == typeof(string))
            {
                fieldInfo.SetValue(currentAsset, SetText(fieldInfo.Name, (string)fieldInfo.GetValue(currentAsset)));
            }
            else if (fieldInfo.FieldType == typeof(int))
            {
                fieldInfo.SetValue(currentAsset, SetInt(fieldInfo.Name, (int)fieldInfo.GetValue(currentAsset)));
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                fieldInfo.SetValue(currentAsset, SetFloat(fieldInfo.Name, (float)fieldInfo.GetValue(currentAsset)));
            }
            else if (fieldInfo.FieldType == typeof(bool))
            {
                fieldInfo.SetValue(currentAsset, SetBool(fieldInfo.Name, (bool)fieldInfo.GetValue(currentAsset)));
            }
            else if (fieldInfo.FieldType == typeof(Vector3))
            {
                fieldInfo.SetValue(currentAsset, SetVector3(fieldInfo.Name, (Vector3)fieldInfo.GetValue(currentAsset)));
            }
            else if (fieldInfo.FieldType.BaseType == typeof(Enum))
            {
                SetEnum(fieldInfo);
                //obj.SetValue(currentAsset, );
            }
            else if (fieldInfo.FieldType == typeof(Texture))
            {
                SetTexture(fieldInfo);
            }
            else if (fieldInfo.FieldType == typeof(Sprite))
            {
                SetSprite(fieldInfo);
            }
            else
            {
                SetList(fieldInfo.Name, (IList)fieldInfo.GetValue(currentAsset), fieldInfo.FieldType);
            }
        });
    }

    public string SetText(string label, string value)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label.ToUpper(), GUILayout.Width(150));
        var retStr = EditorGUILayout.TextField(value);
        GUILayout.EndHorizontal();
        return retStr;
    }

    public int SetInt(string label, int value)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label.ToUpper(), GUILayout.Width(150));
        var retStr = EditorGUILayout.IntField(value);
        GUILayout.EndHorizontal();
        return retStr;
    }

    public float SetFloat(string label, float value)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label.ToUpper(), GUILayout.Width(150));
        var retStr = EditorGUILayout.FloatField(value);
        GUILayout.EndHorizontal();
        return retStr;
    }

    public bool SetBool(string label, bool value)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label.ToUpper(), GUILayout.Width(150));
        var retStr = EditorGUILayout.Toggle(value);
        GUILayout.EndHorizontal();
        return retStr;
    }

    public Vector3 SetVector3(string label, Vector3 value)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label.ToUpper(), GUILayout.Width(150));
        var retStr = EditorGUILayout.Vector3Field("", value);
        GUILayout.EndHorizontal();
        return retStr;
    }

    public void SetList(string label, IList value, Type type)
    {
        ReorderableList list = new ReorderableList(value, type, true, true, true, true);
        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, label.ToUpper());
        };
        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            rect.height -= 4;
            rect.y += 2;
            if (value[index].GetType() == typeof(string))
                value[index] = EditorGUI.TextField(rect, (string)value[index]);
            if (value[index].GetType() == typeof(int))
                value[index] = EditorGUI.IntField(rect, (int)value[index]);
            if (value[index].GetType() == typeof(float))
                value[index] = EditorGUI.FloatField(rect, (float)value[index]);
        };
        list.DoLayoutList();
    }

    public void SetEnum(FieldInfo fieldInfo)
    {
        GUILayout.BeginHorizontal();
        var names = Enum.GetValues(fieldInfo.FieldType);
        GenericMenu menu = new GenericMenu();
        int idx = 0;
        string selectedEnumDropdown = fieldInfo.GetValue(currentAsset).ToString();
        foreach (var name in names)
        {
            var isEquip = name.ToString() == selectedEnumDropdown;
            menu.AddItem(new GUIContent(name.ToString()), isEquip, (nm) =>
            {
                fieldInfo.SetValue(currentAsset, nm);
            }, name);
            idx++;
        }
        GUIContent content = new GUIContent(selectedEnumDropdown);
        EditorGUILayout.LabelField(fieldInfo.Name.ToUpper(), GUILayout.Width(150));
        if (EditorGUILayout.DropdownButton(content, FocusType.Keyboard))
        {
            menu.ShowAsContext();
        }
        GUILayout.EndHorizontal();
    }

    public void SetTexture(FieldInfo fieldInfo)
    {
        var texture = (Texture)fieldInfo.GetValue(currentAsset);
        var rcode = currentAsset.rcode;
        if (currentAsset.GetType().GetField("createItem") != null)
            rcode = (string)currentAsset.GetType().GetField("createItem").GetValue(currentAsset);
        if (currentAsset.GetType().GetField("target") != null)
            rcode = (string)currentAsset.GetType().GetField("target").GetValue(currentAsset);
        var path = DataToolSetting.ThumbnailPath + "/" + rcode + ".png";
        if (texture == null)
        {
            texture = AssetDatabase.LoadAssetAtPath(path, fieldInfo.FieldType) as Texture;
        }
        GUILayout.BeginHorizontal();
        fieldInfo.SetValue(currentAsset, EditorGUILayout.ObjectField(fieldInfo.Name.ToUpper(), texture, fieldInfo.FieldType, false));
        GUILayout.EndHorizontal();
    }

    public void SetSprite(FieldInfo fieldInfo)
    {
        var sprite = (Sprite)fieldInfo.GetValue(currentAsset);
        var rcode = currentAsset.rcode;
        if (currentAsset.GetType().GetField("createItem") != null)
            rcode = (string)currentAsset.GetType().GetField("createItem").GetValue(currentAsset);
        if (currentAsset.GetType().GetField("target") != null)
            rcode = (string)currentAsset.GetType().GetField("target").GetValue(currentAsset);
        var path = DataToolSetting.ThumbnailPath + "/" + rcode + ".png";
        if (sprite == null)
        {
            sprite = AssetDatabase.LoadAssetAtPath(path, fieldInfo.FieldType) as Sprite;
        }
        GUILayout.BeginHorizontal();
        fieldInfo.SetValue(currentAsset, EditorGUILayout.ObjectField(fieldInfo.Name.ToUpper(), sprite, fieldInfo.FieldType, false));
        GUILayout.EndHorizontal();
    }
    #endregion


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

    public async void DownloadData(List<SheetInfoSO> sheets, BaseDataSO data = null)
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

    public void RefreshData(SheetInfoSO sheet, BaseDataSO data)
    {
        var dicData = sheet.datas.Find(obj => obj["rcode"] == data.rcode);
        var path = DataToolSetting.DataScriptableObjectPath + "/" + dicData["rcode"] + ".asset";
        var dt = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, data.GetType());
        dt = TSVParser.DicToSOData(data.GetType(), dt, dicData);

        EditorUtility.SetDirty(dt);
        AssetDatabase.SaveAssets();
    }

    protected void ImportDatas(List<SheetInfoSO> sheets)
    {
        foreach (var sheet in sheets)
        {
            ImportData(sheet);
        }
    }

    protected void ImportData(SheetInfoSO sheet)
    {
        if (sheet.isUpdate)
        {
            Assembly assembly = typeof(BaseDataSO).Assembly;
            var type = assembly.GetType(sheet.className);
            GetDatas(type, sheet.datas);
        }
    }

    public void GetDatas(Type type, List<Dictionary<string, string>> datas)
    {
#if USE_SO_DATA
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
#endif
    }

    private List<SheetInfoSO> sheets { get => DataToolSetting.Instance.sheets; }

#if USE_SO_DATA
    public ScriptableObject DicToClass(Type type, Dictionary<string, string> data)
    {
        var dt = CreateInstance(type);
        AssetDatabase.CreateAsset(dt, DataToolSetting.DataScriptableObjectPath + "/" + data["rcode"] + ".asset");
        return TSVParser.DicToSOData(type, dt, data);
    }
#endif
}
