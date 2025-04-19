using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;

namespace Ironcow.Common
{
    public class InputSettingTool : EditorWindow
    {
        public static InputSettingTool instance;
#if USE_SO_DATA
        [MenuItem("Ironcow/Tool/Input Setting #&i")]
#endif
        public static void Open()
        {
            var window = GetWindow<InputSettingTool>();
            window.minSize = new Vector2(512f, 728f);
            instance = window;
        }

        private Vector2 sceneViewScrollPosition = Vector2.zero;
        private Vector2 windowScrollPosition = Vector2.zero;

        private void OnFocus()
        {
            if (instance == null)
                this.OnEnable();
        }

        private void OnEnable()
        {
            try
            {
                KeyBindListData.Instance.Refresh();
            }
            catch (Exception ex)
            {
                instance = null;
            }
        }

        List<KeyBindData> datas => KeyBindListData.Instance.datas;
        int selectedIdx = 0;
        private void Draw()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            {
                EditorGUI.indentLevel++;
                this.sceneViewScrollPosition = EditorGUILayout.BeginScrollView(this.sceneViewScrollPosition, "box", GUILayout.Width(200));
                {
                    if (GUILayout.Button("키 추가"))
                    {
                        var path = $"{InputSystemSetting.CreateKeyBindPath}KeyBinding{datas.Count + 1}.asset";
                        var data = CreateInstance<KeyBindData>();
                        AssetDatabase.CreateAsset(data, path);
                        KeyBindListData.Instance.Refresh();
                    }
                    var style = new GUIStyle(UnityEngine.GUI.skin.button);
                    style.alignment = TextAnchor.MiddleLeft;
                    int i = 0;
                    foreach(var data in datas)
                    {
                        selectedIdx = GUILayout.Toggle(selectedIdx == i, new GUIContent($"{data.name}({data.keyBind.KeyCode.ToString()})"), style, GUILayout.Width(180)) ? i : selectedIdx;
                        i++;
                    }
                }
                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(10);
            if (selectedIdx >= 0)
            {
                GUILayout.BeginVertical();
                {
                    DrawScriptableObject();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        bool isCheckKey = false;
        public void DrawScriptableObject()
        {
            if (datas.Count == 0) return;
            ScriptableObject scriptableObject = datas[selectedIdx];
            if (GUILayout.Button(isCheckKey ? "키 입력 대기중" : "키 바인딩"))
            {
                isCheckKey = true;
            }
            if (isCheckKey)
            {
                if (Event.current.isKey)
                {
                    datas[selectedIdx].keyBind.KeyCode = Event.current.keyCode;
                    isCheckKey = false;
                }
            }

            scriptableObject = EditorGUILayout.ObjectField("", scriptableObject, scriptableObject.GetType().DeclaringType, true) as ScriptableObject;
            var editor = Editor.CreateEditor(scriptableObject);
            editor.OnInspectorGUI();
            if (KeyBindListData.Instance.keys.Find(obj => obj == datas[selectedIdx].key) == null)
            {
                KeyBindListData.Instance.keys.Add(datas[selectedIdx].key);
                KeyBindListData.Instance.SaveData();
            }
        }

        private void OnGUI()
        {
            this.windowScrollPosition = EditorGUILayout.BeginScrollView(this.windowScrollPosition, "box");
            {
                this.Draw();
            }
            EditorGUILayout.EndScrollView();
        }

    }
}