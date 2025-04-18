using System.Collections;
using System.Collections.Generic;
using Ironcow.ObjectPool;
using UnityEditor;
using UnityEngine;

public class ThisObjectPoolTool : EditorWindow
{
    public static ThisObjectPoolTool instance;

    private List<GameObject> poolPrefabs = new();
    private SerializedObject serializedObject;
    private SerializedProperty prefabListProp;

    [MenuItem("Tool/ObjectPool Tool #&o")]
    public static void Open()
    {
        var window = GetWindow<ThisObjectPoolTool>("Object Pool Tool");
        window.minSize = new Vector2(512f, 728f);
        instance = window;
    }

    private Vector2 scrollPosition;

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("\uD83D\uDE9A 등록할 프리팹 리스트 (IObjectPoolBase만 허용)", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        DrawPrefabDropArea();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, "box");
        {
            for (int i = 0; i < poolPrefabs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                poolPrefabs[i] = (GameObject)EditorGUILayout.ObjectField(poolPrefabs[i], typeof(GameObject), false);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    poolPrefabs.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("\uD83D\uDCBE Save & Convert to ObjectPoolDataSO"))
        {
            ConvertToObjectPoolDataSO();
        }
    }

    private void DrawPrefabDropArea()
    {
        var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "드래그해서 프리팹을 여기에 넣으세요 (IObjectPoolBase만 허용)");

        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is GameObject go && IsValidPoolPrefab(go))
                        {
                            if (!poolPrefabs.Contains(go))
                                poolPrefabs.Add(go);
                        }
                    }
                }
                Event.current.Use();
            }
        }
    }

    private bool IsValidPoolPrefab(GameObject go)
    {
        return go != null && go.GetComponent<IObjectPoolBase>() != null;
    }

    private void ConvertToObjectPoolDataSO()
    {
        var so = ObjectPoolDataSO.instance;
        so.objectPoolDatas.Clear();

        foreach (var prefab in poolPrefabs)
        {
            if (!IsValidPoolPrefab(prefab)) continue;
            var data = new ObjectPoolData
            {
                prefabName = prefab.name,
                count = 10,
                velocity = 25,
                prefab = null,
                parent = null
            };
            so.objectPoolDatas.Add(data);
        }

        so.SaveSO();
        Debug.Log("ObjectPoolDataSO로 변환 및 저장 완료!");
    }

}
