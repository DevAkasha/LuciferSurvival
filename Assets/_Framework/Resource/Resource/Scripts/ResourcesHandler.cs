using Ironcow;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ironcow.Resource
{
    public class ResourcesHandler
    {
        public Dictionary<string, object> assetPools = new Dictionary<string, object>();

        public void Init()
        {

        }

        public T LoadAsset<T>(string key, string type) where T : UnityEngine.Object
        {
            var poolKey = type.ToString() + "/" + key;
            if (assetPools.ContainsKey(poolKey)) return (T)assetPools[poolKey];
            var asset = Resources.Load<T>(poolKey);
            if (asset != null && !assetPools.ContainsKey(poolKey)) assetPools.Add(poolKey, asset);
            return asset;
        }

        public List<T> LoadAssets<T>(string key, string type) where T : UnityEngine.Object
        {
            var path = type.ToString() + "/" + key;
            List<T> retList = new List<T>();
            var keys = new List<string>(assetPools.Keys);
            foreach (var val in keys)
            {
                if (val.Contains(path))
                {
                    retList.Add((T)assetPools[val]);
                }
            }
            if (retList.Count == 0)
            {
                retList = Resources.LoadAll<T>(path).ToList();
                var filename = key.FileName().ToUpper();
                foreach (var item in retList)
                {
                    if (!assetPools.ContainsKey(item.name))
                        assetPools.Add(path + "/" + item.name, item);
                }
            }
            return retList;
        }

#if USE_SO_DATA
        public List<T> LoadDataAssets<T>() where T : UnityEngine.Object
        {
            return LoadAssets<T>("", ResourceType.Datas);
        }
#else
        public List<T> LoadDataAssets<T>()
        {
            var datas = LoadAssets<TextAsset>("Datas", ResourceType.Datas);
            for (int i = 0; i < datas.Count; i++)
            {

            }
            return null;
        }
#endif

        public T LoadThumbnail<T>(string key) where T : UnityEngine.Object
        {
            return LoadAsset<T>(key, ResourceType.Thumbnail);
        }

        public T LoadUI<T>(string key) where T : UnityEngine.Object
        {
            return LoadAsset<T>(key, ResourceType.UI);
        }
    }
}