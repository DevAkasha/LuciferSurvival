using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ironcow;
using UnityEngine;
#if USE_SO_DATA
using Ironcow.Data;
#endif
#if USE_ADDRESSABLE
using Ironcow.Addressable;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.AI;
using UnityEngine;
#endif

namespace Ironcow.Resource
{
    public class ResourceManagerBase<T> : MonoSingleton<T> where T : ResourceManagerBase<T>
    {
        public bool isAutoLoading = false;
        [NonSerialized] public bool isInit = false;

#if !USE_ADDRESSABLE
        private ResourcesHandler handler = new ResourcesHandler();
#elif USE_ADDRESSABLE
        private AddressableHandler handler = new AddressableHandler();
#endif

        public void Init()
        {
#if USE_ADDRESSABLE
            handler.LoadAddressable();
#endif
            isInit = true;
        }

        public V LoadAsset<V>(string key, string type) where V : UnityEngine.Object
        {
            var asset = handler.LoadAsset<V>(key, type);
            return asset;
        }

        public V InstantiateAsset<V>(string key, string type, Transform parent = null) where V : UnityEngine.Object
        {
            var asset = Instantiate(LoadAsset<V>(key, type), parent);
            return asset;
        }

        public V InstantiateAsset<V>(string type, Transform parent = null) where V : UnityEngine.Object
        {
            var asset = Instantiate(LoadAsset<V>(typeof(V).ToString(), type), parent);
            return asset;
        }

        public List<V> LoadAssets<V>(string key, string type) where V : UnityEngine.Object
        {
            var assets = handler.LoadAssets<V>(key, type);
            return assets;
        }

#if USE_SO_DATA
        public List<V> LoadDataAssets<V>() where V : UnityEngine.Object
        {
            return handler.LoadDataAssets<V>();
        }
#else
        public List<V> LoadDataAssets<V>()
        {
            List<V> retList = new List<V>();
            var datas = handler.LoadAssets<TextAsset>("Datas", ResourceType.Datas);
            foreach (var data in datas)
            {
                retList.Add(JsonUtility.FromJson<V>(data.text));
            }
            return retList;
        }
#endif

        public V LoadThumbnail<V>(string key) where V : UnityEngine.Object
        {
            return handler.LoadAsset<V>(key, ResourceType.Thumbnail);
        }

        public V LoadUI<V>(string key) where V : UnityEngine.Object
        {
            return handler.LoadAsset<V>(key, ResourceType.UI);
        }
    }
}