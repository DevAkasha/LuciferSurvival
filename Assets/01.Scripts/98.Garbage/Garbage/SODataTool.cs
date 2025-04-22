using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SODataTool : EditorWindow
{
    private GSpreadReader<DataManager> gspreadReader;
    private List<SheetInfo> sheets => gspreadReader?.Sheets;

    private Vector2 scroll;

    //[MenuItem("Tools/SO Data Tool")]
    public static void OpenWindow()
    {
        var window = GetWindow<SODataTool>("SO Data Tool");
        window.minSize = new Vector2(512f, 400f);
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("📄 GSpreadReader 연결", EditorStyles.boldLabel);

        // 👇 오브젝트 수동 할당
        gspreadReader = (GSpreadReader<DataManager>)EditorGUILayout.ObjectField(
            "GSpreadReader",
            gspreadReader,
            typeof(GSpreadReader<DataManager>),
            true
        );

        EditorGUILayout.Space(10);

        if (gspreadReader == null)
        {
            EditorGUILayout.HelpBox("GSpreadReader<DataManager> 오브젝트를 먼저 선택하세요.", MessageType.Info);
        }
        else
        {
            DrawSheets();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawSheets()
    {
        if (sheets == null || sheets.Count == 0)
        {
            EditorGUILayout.HelpBox("Sheets가 비어있습니다.", MessageType.Warning);
            return;
        }

        EditorGUILayout.LabelField("📑 연결된 Sheets", EditorStyles.boldLabel);

        foreach (var sheet in sheets)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Class Name", sheet.className);
            EditorGUILayout.LabelField("Sheet ID", sheet.sheetId);
            EditorGUILayout.LabelField("데이터 수", (sheet.datas?.Count ?? 0).ToString());
            EditorGUILayout.EndVertical();
        }
    }
}
