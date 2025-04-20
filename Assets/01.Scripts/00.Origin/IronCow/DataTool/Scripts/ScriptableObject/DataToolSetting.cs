using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class DataToolSetting : SOSingleton<DataToolSetting>
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
        [Header("Scriptable Object Data Path")]
        public Object dataScriptableObjectPath;
        public static string DataScriptableObjectPath { get => AssetDatabase.GetAssetPath(Instance.dataScriptableObjectPath); }
        public static string DataScriptableObjectFullPath { get => Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(Instance.dataScriptableObjectPath); }

        [Header("Thumbnail Path")]
        public Object thumbnailPath;
        public static string ThumbnailPath { get => AssetDatabase.GetAssetPath(Instance.thumbnailPath); }

        [Header("Google Sheet Data")]
        public string GSheetUrl;
        public List<SheetInfoSO> sheets;

#endif
    }