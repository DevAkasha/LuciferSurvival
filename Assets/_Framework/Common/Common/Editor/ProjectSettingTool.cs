using Ironcow.Data;
using Ironcow.LocalizeTool;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using Ironcow.Convenience;
using Ironcow.ThumbnailMaker;
using Ironcow.ObjectPool;
using Ironcow.Tool3D;
using Ironcow.Network;
using Ironcow.Resource;
using Ironcow.Sound;
using Ironcow.JoyStick;
using Ironcow.ProjectTool;
using Ironcow.UI;
using System.Reflection;

namespace Ironcow.Common
{
    public class ProjectSettingTool : EditorWindow
    {
        public static ProjectSettingTool instance;
        [MenuItem("Ironcow/Tool/Project Setting #&p")]
        public static void Open()
        {
            var window = GetWindow<ProjectSettingTool>();
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
            }
            catch (Exception ex)
            {
                instance = null;
            }
        }

        List<string> datas = new List<string>();
        int isSelected = 0;
        bool isDataToolOpened = false;
        private void Draw()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            {
                EditorGUI.indentLevel++;
                this.sceneViewScrollPosition = EditorGUILayout.BeginScrollView(this.sceneViewScrollPosition, "box", GUILayout.Width(200));
                {
                    var style = new GUIStyle(UnityEngine.GUI.skin.button);
                    style.alignment = TextAnchor.MiddleLeft;
                    isSelected = GUILayout.Toggle(isSelected == 0, new GUIContent("Framework Controller"), style, GUILayout.Width(180)) ? 0 : isSelected;
                    isSelected = GUILayout.Toggle(isSelected == 1, new GUIContent("Application Setting"), style, GUILayout.Width(180)) ? 1 : isSelected;
                    isSelected = GUILayout.Toggle(isSelected == 2, new GUIContent("Editor Setting"), style, GUILayout.Width(180)) ? 2 : isSelected;
                    isSelected = GUILayout.Toggle(isSelected == 3, new GUIContent("Thumbnail Maker Setting"), style, GUILayout.Width(180)) ? 3 : isSelected;
                    isSelected = GUILayout.Toggle(isSelected == 4, new GUIContent("Input System Setting"), style, GUILayout.Width(180)) ? 4 : isSelected;
#if USE_SO_DATA
                    isSelected = GUILayout.Toggle(isSelected == 5, new GUIContent("Name Changer"), style, GUILayout.Width(180)) ? 5 : isSelected;
#endif
                    if (FrameworkController.Instance.isScriptableObjectData)
                        isSelected = GUILayout.Toggle(isSelected == 101, new GUIContent("Data Tool Setting"), style, GUILayout.Width(180)) ? 101 : isSelected;
                    if (FrameworkController.Instance.isLocale)
                        isSelected = GUILayout.Toggle(isSelected == 102, new GUIContent("Localize Path Setting"), style, GUILayout.Width(180)) ? 102 : isSelected;
                    if (FrameworkController.Instance.isObjectPool)
                        isSelected = GUILayout.Toggle(isSelected == 103, new GUIContent("ObjectPool Tool Setting"), style, GUILayout.Width(180)) ? 103 : isSelected;
                    if (FrameworkController.Instance.isFSM)
                        isSelected = GUILayout.Toggle(isSelected == 104, new GUIContent("FSM Create Tool"), style, GUILayout.Width(180)) ? 104 : isSelected;
                }
                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(10);
            if (isSelected >= 0)
            {
                GUILayout.BeginVertical();
                {
                    DrawScriptableObject();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        public void DrawScriptableObject()
        {
            ScriptableObject scriptableObject;
            switch(isSelected)
            {
                case 0:
                    {
                        scriptableObject = FrameworkController.Instance;
                    }
                    break;
                case 1:
                    {
                        scriptableObject = ApplicationSettings.Instance;
                    }
                    break;
                case 2:
                    {
                        scriptableObject = EditorDataSetting.Instance;
                        DrawEditorButtons();
                    }
                    break;
                case 3:
                    {
                        scriptableObject = ThumbnailPathSetting.Instance;
                    }
                    break;
                case 4:
                    {
                        if (GUILayout.Button("Open Input System Tool"))
                        {
                            InputSettingTool.Open();
                        }
                        var obj = EditorGUILayout.ObjectField("", InputSystemSetting.Instance, InputSystemSetting.Instance.GetType().DeclaringType, true) as ScriptableObject;
                        var editor2 = Editor.CreateEditor(InputSystemSetting.Instance);
                        editor2.OnInspectorGUI();
                        GUILayout.Space(30);
                        scriptableObject = KeyBindListData.Instance;
                    }
                    break;
                case 5:
                    {
                        scriptableObject = NameChanger.Instance;
                    }
                    break;
                case 101:
                    {
                        scriptableObject = DataToolSetting.Instance;
                        if (GUILayout.Button("Open Data Tool"))
                        {
                            DataTool.Open();
                        }
                    }
                    break;
                case 102:
                    {
                        scriptableObject = LocalePathSetting.Instance;
                        if (GUILayout.Button("Open Localize Tool"))
                        {
                            LocalizeTool.LocalizeTool.Open();
                        }
                    }
                    break;
                case 103:
                    {
                        scriptableObject = ObjectPoolPathSetting.Instance;
                        if (GUILayout.Button("Open ObjectPool Tool"))
                        {
                            ObjectPoolTool.Open();
                        }
                    }
                    break;
                case 104:
                    {
                        scriptableObject = FSMData.Instance;
                    }
                    break;
                default:
                    scriptableObject = FrameworkController.Instance;
                    break;
            }

            scriptableObject = EditorGUILayout.ObjectField("", scriptableObject, scriptableObject.GetType().DeclaringType, true) as ScriptableObject;
            var editor = Editor.CreateEditor(scriptableObject);
            editor.OnInspectorGUI();
        }

        private void OnGUI()
        {
            this.windowScrollPosition = EditorGUILayout.BeginScrollView(this.windowScrollPosition, "box");
            {
                this.Draw();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawEditorButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Partial Scripts"))
            {
                if (Type.GetType("Ironcow.Data.DataManagerBase") != null)
                {
                    DataEditor.CreatePartialScripts();
                }
                if (Type.GetType("Ironcow.Resource.ResourceManagerBase") != null)
                {
                    ResourceEditor.CreatePartialScripts();
                }
            }
            if (GUILayout.Button("Create Setting SOs"))
            {
                if (Type.GetType("Ironcow.Data.DataEditor") != null)
                {
                    ObjectPoolEditor.CreateSettingSO();
                }
                if (Type.GetType("Ironcow.Input.KeyBindDrawer") != null)
                {
                    //InputManagerEditor.CreateSettingSO();
                }
            }
            if (GUILayout.Button("Create Manager Instances"))
            {
                if (Type.GetType("Ironcow.Data.DataEditor") != null)
                {
                    DataEditor.CreateManagerInstance();
                }
                if (Type.GetType("Ironcow.Tool3D.Tool3DEditor") != null)
                {
                    Tool3DEditor.CreateManagerInstance();
                }
                if (Type.GetType("Ironcow.Resource.ResourceEditor") != null)
                {
                    ResourceEditor.CreateManagerInstance();
                }
                if (Type.GetType("Ironcow.Network.NetworkEditor") != null)
                {
                    NetworkEditor.CreateManagerInstance();
                }
                if (Type.GetType("Ironcow.Sound.SoundEditor") != null)
                {
                    SoundEditor.CreateManagerInstance();
                }
                if (Type.GetType("Ironcow.ObjectPool.ObjectPoolEditor") != null)
                {
                    ObjectPoolEditor.CreateManagerInstance();
                }
                if (Type.GetType("Ironcow.UI.UIEditor") != null)
                {
                    UIEditor.CreateManagerInstance();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Asset Prefabs"))
            {
                if (Type.GetType("Ironcow.ObjectPool.ObjectPoolEditor") != null)
                {
                    ObjectPoolEditor.CreatePrefab();
                }
            }
            if (GUILayout.Button("Create Joypad Prefabs"))
            {
                if (Type.GetType("Ironcow.JoyStick.JoyStickEditor") != null)
                {
                    JoyStickEditor.CreatePrefab();
                }
            }
            GUILayout.EndHorizontal();
        }

    }
}