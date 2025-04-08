using UnityEngine;
using UnityEditor;
using System.IO;
using Ironcow.Convenience;
using Ironcow.Sound;

namespace Ironcow.ObjectPool
{
	public class ObjectPoolEditor : Editor
    {
        public static void CreateSettingSO()
        {
            var outPath = Application.dataPath.Replace("Assets", EditorDataSetting.SettingSOPath);
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
            AssetDatabase.Refresh();
            outPath = outPath.Replace(Application.dataPath, "Assets");
            var filePath = outPath + "/ObjectPoolSetting.asset";
            if (!AssetDatabase.LoadAssetAtPath(filePath, typeof(Object)))
            {
                var instance = CreateInstance<ObjectPoolDataSO>();
                AssetDatabase.CreateAsset(instance, filePath);
            }
        }

        public static void CreateManagerInstance()
        {
            if (GameObject.Find("PoolManager")) return;
            var obj = new GameObject("PoolManager");
            obj.AddComponent<PoolManager>();
        }

        public static void CreatePrefab()
        {
            var outPath = Application.dataPath.Replace("Assets", EditorDataSetting.CreateAssetPrefabPath);
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
            AssetDatabase.Refresh();
            outPath = outPath.Replace(Application.dataPath, "Assets");
            var filePath = outPath + "/AudioSource.Prefab";
            if (!AssetDatabase.LoadAssetAtPath(filePath, typeof(Object)))
            {
                var obj = new GameObject();
                var data = obj.AddComponent<AudioSourcePoolData>();
                data.SetSource(obj.AddComponent<AudioSource>());
                
                PrefabUtility.SaveAsPrefabAsset(obj, filePath);
                DestroyImmediate(obj);
            }
        }
    }
}