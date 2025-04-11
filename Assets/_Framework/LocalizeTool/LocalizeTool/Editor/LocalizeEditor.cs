using UnityEngine;
using UnityEditor;
using System.IO;

namespace Ironcow.LocalizeTool
{
	public class LocalizeEditor : Editor
    {

        [MenuItem("Ironcow/Setting/Create LocalizeToolSetting")]
        private static void CreateSettingSO()
        {
            var outPath = Application.dataPath + @"\_Ironcow\Setting\Resources\";
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
            AssetDatabase.Refresh();
            outPath = outPath.Replace(Application.dataPath, "Assets");
            var filePath = outPath + "LocalizeToolSetting.asset";
            if (AssetDatabase.LoadAssetAtPath(filePath, typeof(Object)))
            {
                var instance = CreateInstance<LocalePathSetting>();
                //PrefabUtility.SaveAsPrefabAsset(instance.controller, filePath);
                AssetDatabase.CreateAsset(instance, filePath);
            }
        }
    }
}