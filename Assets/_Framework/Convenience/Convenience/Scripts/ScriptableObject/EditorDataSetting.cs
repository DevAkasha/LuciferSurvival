using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ironcow.Convenience
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class EditorDataSetting : SOSingleton<EditorDataSetting>
    {
#if UNITY_EDITOR
        public static string ScriptFolderFullPath { get; private set; }      // "......\이 스크립트가 위치한 폴더 경로"
        public static string ScriptFolderInProjectPath { get; private set; } // "Assets\...\이 스크립트가 위치한 폴더 경로"
        public static string AssetFolderPath { get; private set; }

        private static void InitFolderPath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            ScriptFolderFullPath = System.IO.Path.GetDirectoryName(sourceFilePath);
            int rootIndex = ScriptFolderFullPath.IndexOf(@"Assets\");
            if (rootIndex > -1)
            {
                ScriptFolderInProjectPath = ScriptFolderFullPath.Substring(rootIndex, ScriptFolderFullPath.Length - rootIndex);
            }
        }
        [Header("Prefab Path")]
        public List<Object> prefabFolders;

        [Header("Scene Path")]
        public Object scenePath;
        public static string ScenePath { get => AssetDatabase.GetAssetPath(instance.scenePath); }

        public Object editorScenePath;
        public static string EditorScenePath { get => AssetDatabase.GetAssetPath(instance.editorScenePath); }

        public Object introScene;
        public static string IntroScenePath { get => AssetDatabase.GetAssetPath(instance.introScene); }

        public Object dontDestroyScene;
        public static string DontDestroyScenePath { get => AssetDatabase.GetAssetPath(instance.dontDestroyScene); }

        [Header("Script Templete Path")]
        public Object templetePath;
        public static string TempletePath { get => AssetDatabase.GetAssetPath(instance.templetePath); }

        [Header("Create Prefab Path")]
        public Object createPrefabPath;
        public static string CreatePrefabPath { get => AssetDatabase.GetAssetPath(instance.createPrefabPath); }

        [Header("Create SettingSO Path")]
        public Object settingSOPath;
        public static string SettingSOPath { get => AssetDatabase.GetAssetPath(instance.settingSOPath); }

        [Header("Create Script Path")]
        public Object scriptPath;
        public static string ScriptPath { get => AssetDatabase.GetAssetPath(instance.scriptPath); }

        [Header("Create Asset Prefab Path")]
        public Object createAssetPrefabPath;
        public static string CreateAssetPrefabPath { get => AssetDatabase.GetAssetPath(instance.createAssetPrefabPath); }
#endif
    }
}