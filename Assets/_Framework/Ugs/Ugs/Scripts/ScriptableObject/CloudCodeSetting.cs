using Ironcow;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ironcow.Ugs
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class CloudCodeSetting : SOSingleton<CloudCodeSetting>
    {
#if UNITY_EDITOR
        public static string ScriptFolderFullPath { get; private set; }      // "......\�� ��ũ��Ʈ�� ��ġ�� ���� ���"
        public static string ScriptFolderInProjectPath { get; private set; } // "Assets\...\�� ��ũ��Ʈ�� ��ġ�� ���� ���"
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
        [Header("Java Script Templete Path")]
        public Object jsTempletePath;
        public static string JSTempletePath { get => AssetDatabase.GetAssetPath(instance.jsTempletePath); }
        public static string JSTempleteFullPath { get => Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.jsTempletePath); }

        [Header("Java Scripts Path")]
        public Object jsPath;
        public static string JSPath { get => AssetDatabase.GetAssetPath(instance.jsPath); }
        public static string JSFullPath { get => Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.jsPath); }

        [Header("C# Network Code Path")]
        public Object networkPath;
        public static string NetworkPath { get => AssetDatabase.GetAssetPath(instance.networkPath); }

#endif
    }
}