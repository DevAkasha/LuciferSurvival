using Ironcow;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ironcow.LocalizeTool
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class LocalePathSetting : SOSingleton<LocalePathSetting>
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

        [Header("Localize Prefab Path")]
        public List<Object> prefabFolders;

        [Header("Localize")]
        public string GSheetUrl;
        public string localeSheetId;
        public Object localeDataPath;
        public static string LocaleDataPath { get => AssetDatabase.GetAssetPath(instance.localeDataPath); }
#endif
    }
}