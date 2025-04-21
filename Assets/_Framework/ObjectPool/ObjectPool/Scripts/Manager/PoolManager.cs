using System.Collections.Generic;
using UnityEngine;

namespace Ironcow.ObjectPool
{
   
    public class PoolManager : MonoSingleton<PoolManager>
    {
        Dictionary<string, Queue<IObjectPoolBase>> pools = new Dictionary<string, Queue<IObjectPoolBase>>();
        public bool isInit = false;

        public void Init()
        {
            
            foreach (var data in ObjectPoolDataSO.instance.objectPoolDatas)
            {
                var prefabGO = ResourceManager.instance.LoadAsset<GameObject>($"GameDatas/ObjectPoolData/Enemy", ResourceType.Prefabs);

                if (prefabGO == null)
                {
                    Debug.LogError($"❌ 프리팹 로드 실패: {data.prefabName} (경로나 파일명이 맞는지 확인하세요)");
                    return;
                }
                data.prefab = ResourceManager.instance.LoadAsset<GameObject>(data.prefabName, ResourceType.Prefabs).GetComponent<IObjectPoolBase>();
                data.parent = new GameObject(data.prefabName + "parent").transform;
                data.parent.parent = transform;
                Queue<IObjectPoolBase> queue = new Queue<IObjectPoolBase>();
                pools.Add(data.prefabName, queue);
                for (int i = 0; i < data.count; i++)
                {
                    var obj = Instantiate(data.prefab as MonoBehaviour, data.parent);
                    obj.name = obj.name.Replace("(Clone)", "");
                    obj.SetActive(false);
                    queue.Enqueue(obj as IObjectPoolBase);
                }
            }
            isInit = true;
        }

        public T Spawn<T>(string rcode, params object[] param) where T : IObjectPoolBase
        {
            if (pools[rcode].Count == 0)
            {
                var data = ObjectPoolDataSO.instance.objectPoolDatas.Find(obj => obj.prefabName == rcode);
                for (int i = 0; i < data.count; i++)
                {
                    var obj = Instantiate(data.prefab as  MonoBehaviour, data.parent);
                    obj.name.Replace("(Clone)", "");
                    pools[rcode].Enqueue(obj as IObjectPoolBase);
                }
            }
            var retObj = (T)pools[rcode].Dequeue();
            retObj.SetActive(true);
            retObj.Init(param);
            return retObj;
        }

        public T Spawn<T>() where T : IObjectPoolBase
        {
            var retObj = Spawn<T>(typeof(T).ToString());
            return retObj;
        }

        public T Spawn<T>(string rcode, Transform parent, params object[] param) where T : IObjectPoolBase
        {
            var obj = Spawn<T>(rcode, param);
            (obj as MonoBehaviour).transform.parent = parent;
            return obj;
        }

        public T Spawn<T>(Transform parent) where T : IObjectPoolBase
        {
            var obj = Spawn<T>();
            (obj as MonoBehaviour).transform.parent = parent;
            return obj;
        }

        public T Spawn<T>(string rcode, Vector3 position, Transform parent, params object[] param) where T : IObjectPoolBase
        {
            var obj = Spawn<T>(rcode, parent, param);
            (obj as MonoBehaviour).transform.position = position;
            return obj;
        }

        public T Spawn<T>(Vector3 position, Transform parent) where T : IObjectPoolBase
        {
            var obj = Spawn<T>(parent);
            (obj as MonoBehaviour).transform.position = position;
            return obj;
        }

        public T Spawn<T>(string rcode, Vector3 position, Quaternion rotation, Transform parent, params object[] param) where T : IObjectPoolBase
        {
            var obj = Spawn<T>(rcode, position, parent, param);
            (obj as MonoBehaviour).transform.rotation = rotation;
            return obj;
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent) where T : IObjectPoolBase
        {
            var obj = Spawn<T>(position, parent);
            (obj as MonoBehaviour).transform.rotation = rotation;
            return obj;
        }

        public T SpawnAudioSource<T>() where T : IObjectPoolBase
        {
            if (pools["AudioSource"].Count == 0)
            {
                var data = ObjectPoolDataSO.instance.objectPoolDatas.Find(obj => obj.prefabName == "AudioSource");
                for (int i = 0; i < data.count; i++)
                {
                    var obj = Instantiate(data.prefab as MonoBehaviour, data.parent);
                    obj.name.Replace("(Clone)", "");
                    pools["AudioSource"].Enqueue(obj as IObjectPoolBase);
                }
            }
            var retObj = (T)pools["AudioSource"].Dequeue();
            retObj.SetActive(true);
            return retObj;
        }

        public void Release(IObjectPoolBase item)
        {
            item.SetActive(false);
            var data = ObjectPoolDataSO.instance.objectPoolDatas.Find(obj => obj.prefabName == (item as MonoBehaviour).name);
            (item as MonoBehaviour).transform.parent = data.parent;
            pools[(item as MonoBehaviour).name].Enqueue(item);
        }
    }
}