using Ironcow;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
#if USE_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif
using UnityEngine.SceneManagement;
using System.Collections;
using Ironcow.Resource;
using Ironcow.UI;

namespace Ironcow.Addressable
{
    public class AddressableHandler
    {
        private Dictionary<string, Dictionary<string, AddressableMap>> addressableMap = new Dictionary<string, Dictionary<string, AddressableMap>>();

        private void InitAddressableMap()
        {
#if USE_ADDRESSABLE
            Addressables.LoadAssetsAsync<TextAsset>("AddressableMap", (text) =>
            {
                var map = JsonUtility.FromJson<AddressableMapData>(text.text);
                var key = ResourceType.Prefabs;
                Dictionary<string, AddressableMap> mapDic = new Dictionary<string, AddressableMap>();
                foreach (var data in map.list)
                {
                    key = data.addressableType;
                    if (!mapDic.ContainsKey(data.key) && data.key != "addressableMap")
                        mapDic.Add(data.key.ToLower(), data);
                }
                if (!addressableMap.ContainsKey(key)) addressableMap.Add(key, mapDic);

            }).WaitForCompletion();
#endif
        }

        public void PreloadUIs()
        {
            LoadAssets<UIBase>("", ResourceType.UI);
        }

        public void LoadAddressable()
        {
#if USE_ADDRESSABLE
            var init = Addressables.InitializeAsync().WaitForCompletion();
            Addressables.DownloadDependenciesAsync("InitDownload").WaitForCompletion();
#endif
            InitAddressableMap();
            

        }

#if USE_ADDRESSABLE
        public IEnumerator SetProgress(AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                UILoading.instance.SetProgress(handle.GetDownloadStatus().Percent, "Resource Download...");
                yield return new WaitForEndOfFrame();
            }
            UILoading.instance.SetProgress(1);

        }
#endif

        public List<string> GetPaths(string key, string addressableType)
        {
            var keys = new List<string>(addressableMap[addressableType].Keys);
            keys.RemoveAll(obj => !obj.Contains(key));
            List<string> retList = new List<string>();
            keys.ForEach(obj =>
            {
                retList.Add(addressableMap[addressableType][obj].path);
            });
            return retList;
        }
        public string GetPath(string key, string addressableType)
        {
            var newKey = key.Split('/').Last().ToLower();
            var map = addressableMap[addressableType][newKey];
            return map.path;
        }

        public List<T> LoadAssets<T>(string key, string addressableType)
        {
            try
            {
                var paths = GetPaths(key, addressableType);
                List<T> retList = new List<T>();
                foreach (var path in paths)
                {
                    retList.Add(LoadAssetAsync<T>(path));
                }
                return retList;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return default;
        }

        public List<T> LoadDataAssets<T>()
        {
#if USE_ADDRESSABLE
            try
            {
                var retList = new List<T>(Addressables.LoadAssetsAsync<T>("Data").WaitForCompletion());
                return retList;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
#endif
            return default;
        }


        public T LoadAsset<T>(string key, string addressableType)
        {
            try
            {
                var path = GetPath(key, addressableType);
                return LoadAssetAsync<T>(path);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
            return default;
        }
        private T LoadAssetAsync<T>(string path)
        {

#if USE_ADDRESSABLE
            try
            {
                if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
                {
                    var obj = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
                    return obj.GetComponent<T>();
                }
                else
                    return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
#endif
            return default;
        }

        public void LoadScene(string key, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            try
            {
                var path = GetPath(key, ResourceType.Scene);
#if USE_ADDRESSABLE
                var scene = Addressables.LoadSceneAsync(path, mode).WaitForCompletion();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}