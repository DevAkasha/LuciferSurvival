using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Ironcow.Data;

namespace Ironcow.ObjectPool
{
    public class ObjectPoolTool : EditorWindow
    {

        public static ObjectPoolTool instance;
#if USE_OBJECT_POOL
        [MenuItem("Ironcow/Tool/ObjectPool Tool #&o")]
#endif
        public static void Open()
        {
            var window = GetWindow<ObjectPoolTool>();
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

        }

        private void OnDestroy()
        {
            ObjectPoolDataSO.Instance.SaveSO();
        }

        int selectIndex = 0;
        public ObjectPoolData poolData;
        Dictionary<string, bool> isOpened = new Dictionary<string, bool>();
        private void Draw()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            {
                GUILayout.BeginVertical();
                if (GUILayout.Button("Add new pool data"))
                {
                    ObjectPoolDataSO.Instance.objectPoolDatas.Add(new ObjectPoolData());
                    ObjectPoolDataSO.Instance.SaveSO();
                }
                EditorGUI.indentLevel++;
                this.sceneViewScrollPosition = EditorGUILayout.BeginScrollView(this.sceneViewScrollPosition, "box", GUILayout.Width(200));
                {
                    int index = 0;
                    foreach (var data in ObjectPoolDataSO.Instance.objectPoolDatas)
                    {
                        var name = data.prefabName;
                        if (data != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            var style = new GUIStyle(UnityEngine.GUI.skin.button);
                            style.alignment = TextAnchor.MiddleRight;
                            var selection = GUILayout.Toggle(selectIndex == index, string.IsNullOrEmpty(name) ? " " : name, style, GUILayout.Width(180));
                            if (selection) selectIndex = index;
                            EditorGUILayout.EndHorizontal();
                        }
                        index++;
                    }
                }
                GUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(10);
            if (ObjectPoolDataSO.Instance.objectPoolDatas != null && ObjectPoolDataSO.Instance.objectPoolDatas.Count > 0)
            {
                poolData = ObjectPoolDataSO.Instance.objectPoolDatas[selectIndex];
                if (poolData != null)
                {
                    GUILayout.BeginVertical();
                    {
                        var fields = poolData.GetType().GetFields().ToList();
                        fields.ForEach(obj =>
                        {
                            if (obj.FieldType == typeof(string))
                            {
                                obj.SetValue(poolData, SetText(obj.Name, (string)obj.GetValue(poolData)));
                            }
                            else if (obj.FieldType == typeof(int))
                            {
                                obj.SetValue(poolData, SetInt(obj.Name, (int)obj.GetValue(poolData)));
                            }
#if !USE_ADDRESSABLE
                            else if (obj.FieldType == typeof(ObjectPoolBase))
                            {
                                SetObject(obj);
                            }
                            else if (obj.FieldType == typeof(UnityEngine.Object))
                            {
                                SetObject(obj);
                            }
#endif
                        });
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
            ObjectPoolDataSO.Instance.SaveSO();
        }


        public string SetText(string label, string value)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label.FirstCharacterToUpper(), GUILayout.Width(150));
            var retStr = EditorGUILayout.TextField(value);
            GUILayout.EndHorizontal();
            return retStr;
        }

        public int SetInt(string label, int value)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label.FirstCharacterToUpper(), GUILayout.Width(150));
            var retStr = EditorGUILayout.IntField(value);
            GUILayout.EndHorizontal();
            return retStr;
        }

        public void SetObject(FieldInfo fieldInfo)
        {
            var obj = (ObjectPoolBase)fieldInfo.GetValue(poolData);
            var rcode = poolData.prefabName;
            if (poolData.GetType().GetField("createItem") != null)
                rcode = (string)poolData.GetType().GetField("createItem").GetValue(poolData);
            if (poolData.GetType().GetField("target") != null)
                rcode = (string)poolData.GetType().GetField("target").GetValue(poolData);
            var path = DataToolSetting.ThumbnailPath + "/" + rcode + ".png";
            if (obj == null)
            {
                obj = AssetDatabase.LoadAssetAtPath(path, fieldInfo.FieldType) as ObjectPoolBase;
            }
            GUILayout.BeginHorizontal();
            fieldInfo.SetValue(poolData, EditorGUILayout.ObjectField(fieldInfo.Name.FirstCharacterToUpper(), obj, fieldInfo.FieldType, false));
            GUILayout.EndHorizontal();
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