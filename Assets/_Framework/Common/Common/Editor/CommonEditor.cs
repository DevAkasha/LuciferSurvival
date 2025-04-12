using UnityEngine;
using UnityEditor;
using System.IO;

namespace Ironcow
{
	public class CommonEditor : Editor
    {
        public static string ScriptFolderFullPath { get; private set; }      // "......\이 스크립트가 위치한 폴더 경로"
        public static string ScriptFolderInProjectPath { get; private set; } // "Assets\...\이 스크립트가 위치한 폴더 경로"
        public static string AssetFolderPath { get; private set; }           // "....../Assets"

        private static void CreateSettingSO()
        {
            var outPath = Application.dataPath + @"\_Ironcow\Setting\Resources\";
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
            AssetDatabase.Refresh();
            outPath = outPath.Replace(Application.dataPath, "Assets");
            var filePath = outPath + "FrameworkController.asset";
            if (AssetDatabase.LoadAssetAtPath(filePath, typeof(Object)))
            {
                var instance = CreateInstance<FrameworkController>();
                //PrefabUtility.SaveAsPrefabAsset(instance.controller, filePath);
                AssetDatabase.CreateAsset(instance, filePath);
            }
        }
    }
}