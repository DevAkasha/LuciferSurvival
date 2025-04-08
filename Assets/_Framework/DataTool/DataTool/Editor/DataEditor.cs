using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Ironcow.Convenience;

namespace Ironcow.Data
{
    public class DataEditor : Editor
    {
        private static DataEditor _instance;
        public static DataEditor instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new DataEditor();
                }
                return _instance;
            }
        }

        [SerializeField] public TextAsset templete;
        [SerializeField] public TextAsset userInfoTemplete;

        public static void CreatePartialScripts()
        {
            var outPath = Application.dataPath.Replace("Assets", EditorDataSetting.ScriptPath);
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
            AssetDatabase.Refresh();
            var path = outPath + "/Common/";
            var filePath = path + "UserInfo.cs";
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var file = File.Create(filePath);
                var bytes = Encoding.UTF8.GetBytes(instance.userInfoTemplete.text);
                file.Write(bytes, 0, bytes.Length);
                file.Dispose();
                file.Close();
                AssetDatabase.Refresh();
            }
            path = outPath + "/Manager/";
            filePath = path + "DataManager.cs";
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var file = File.Create(filePath);
                var bytes = Encoding.UTF8.GetBytes(instance.templete.text);
                file.Write(bytes, 0, bytes.Length);
                file.Dispose();
                file.Close();
                AssetDatabase.Refresh();
            }
        }

        public static void CreateManagerInstance()
        {
            if (GameObject.Find("DataManager")) return;
            var manager = new GameObject("DataManager");
            manager.AddComponent<DataManager>();
        }
    }
}