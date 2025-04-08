using Ironcow.Convenience;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Ironcow.JoyStick
{
    public class JoyStickEditor : Editor
    {
        private static JoyStickEditor _instance;
        public static JoyStickEditor instance
        {
            get
            {
                if (_instance == null)
                    _instance = new JoyStickEditor();
                return _instance;
            }
        }

        [SerializeField] public GameObject prefab;
        public static string ScriptFolderFullPath { get; private set; }      // "......\이 스크립트가 위치한 폴더 경로"
        public static string ScriptFolderInProjectPath { get; private set; } // "Assets\...\이 스크립트가 위치한 폴더 경로"
        public static string AssetFolderPath { get; private set; }           // "....../Assets"

        public static void CreatePrefab()
        {
            InitFolderPath();
            var prefabPath = ScriptFolderInProjectPath + @"\Prefabs\VirtualStick.prefab";
            Debug.Log(prefabPath);
            var stick = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (stick == null)
            {
                stick = instance.prefab;
            }
            if (stick != null)
            {
                var outPath = Application.dataPath.Replace("Assets", EditorDataSetting.CreatePrefabPath);
                if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
                AssetDatabase.Refresh();
                outPath = outPath.Replace(Application.dataPath, "Assets");
                var filePath = outPath + "/" + stick.name + ".prefab";
                if (!AssetDatabase.LoadAssetAtPath(filePath, typeof(Object)))
                {
                    PrefabUtility.SaveAsPrefabAsset(stick, filePath);
                }
            }
            else
            {
                Debug.Log("stick is null");
            }
        }


        private static void InitFolderPath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            ScriptFolderFullPath = System.IO.Path.GetDirectoryName(sourceFilePath);
            int rootIndex = ScriptFolderFullPath.IndexOf(@"Assets\");
            if (rootIndex > -1)
            {
                ScriptFolderInProjectPath = ScriptFolderFullPath.Substring(rootIndex, ScriptFolderFullPath.Length - rootIndex).Replace(@"\Editor", "");
            }
        }
    }
}