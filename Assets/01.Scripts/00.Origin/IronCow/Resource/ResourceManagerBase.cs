using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine.Events;
using UnityEngine;

#if USE_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.AI;
#endif

public class ResourceManagerBase<T> : ManagerBase<T> where T : ResourceManagerBase<T>
{
    public bool isAutoLoading = false;

#if !USE_ADDRESSABLE
    private ResourcesHandler handler = new ResourcesHandler();
#elif USE_ADDRESSABLE
        private AddressableHandler handler = new AddressableHandler();
#endif

    public async override void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
    {
#if USE_ADDRESSABLE
            await handler.LoadAddressable(progressTextCallback, progressValueCallback);
#endif
        isInit = true;
    }

#if USE_ADDRESSABLE
        public void InitAddressableMap()
        {
            handler.InitAddressableMap();
        }
#endif

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

    public List<V> LoadDataAssets<V>() where V : UnityEngine.Object
    {
        return handler.LoadDataAssets<V>();
    }

    public V LoadThumbnail<V>(string key) where V : UnityEngine.Object
    {
        return handler.LoadAsset<V>(key, ResourceType.Thumbnail);
    }

    public V LoadUI<V>(string key) where V : UnityEngine.Object
    {
        return handler.LoadAsset<V>(key, ResourceType.UI);
    }
}
