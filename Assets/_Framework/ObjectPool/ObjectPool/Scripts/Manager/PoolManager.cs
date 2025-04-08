using System.Collections.Generic;
using UnityEngine;

namespace Ironcow.ObjectPool
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        Dictionary<string, Queue<ObjectPoolBase>> pools = new Dictionary<string, Queue<ObjectPoolBase>>();
        public bool isInit = false;

        public void Init()
        {
            foreach (var data in ObjectPoolDataSO.Instance.objectPoolDatas)
            {
                data.prefab = ResourceManager.instance.LoadAsset<ObjectPoolBase>(data.prefabName, ResourceType.Prefabs);
                data.parent = new GameObject(data.prefabName + "parent").transform;
                data.parent.parent = transform;
                Queue<ObjectPoolBase> queue = new Queue<ObjectPoolBase>();
                pools.Add(data.prefabName, queue);
                for (int i = 0; i < data.count; i++)
                {
                    var obj = Instantiate(data.prefab, data.parent);
                    obj.name = obj.name.Replace("(Clone)", "");
                    obj.SetActive(false);
                    queue.Enqueue(obj);
                }
            }
            isInit = true;
        }

        public T Spawn<T>(string rcode, params object[] param) where T : ObjectPoolBase
        {
            if (pools[rcode].Count == 0)
            {
                var data = ObjectPoolDataSO.Instance.objectPoolDatas.Find(obj => obj.prefabName == rcode);
                for (int i = 0; i < data.count; i++)
                {
                    var obj = Instantiate(data.prefab, data.parent);
                    obj.name.Replace("(Clone)", "");
                    pools[rcode].Enqueue(obj);
                }
            }
            var retObj = (T)pools[rcode].Dequeue();
            retObj.SetActive(true);
            retObj.Init(param);
            return retObj;
        }

        public T Spawn<T>() where T : ObjectPoolBase
        {
            var retObj = Spawn<T>(typeof(T).ToString());
            return retObj;
        }

        public T Spawn<T>(string rcode, Transform parent, params object[] param) where T : ObjectPoolBase
        {
            var obj = Spawn<T>(rcode, param);
            obj.transform.parent = parent;
            return obj;
        }

        public T Spawn<T>(Transform parent) where T : ObjectPoolBase
        {
            var obj = Spawn<T>();
            obj.transform.parent = parent;
            return obj;
        }

        public T Spawn<T>(string rcode, Vector3 position, Transform parent, params object[] param) where T : ObjectPoolBase
        {
            var obj = Spawn<T>(rcode, parent, param);
            obj.transform.position = position;
            return obj;
        }

        public T Spawn<T>(Vector3 position, Transform parent) where T : ObjectPoolBase
        {
            var obj = Spawn<T>(parent);
            obj.transform.position = position;
            return obj;
        }

        public T Spawn<T>(string rcode, Vector3 position, Quaternion rotation, Transform parent, params object[] param) where T : ObjectPoolBase
        {
            var obj = Spawn<T>(rcode, position, parent, param);
            obj.transform.rotation = rotation;
            return obj;
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent) where T : ObjectPoolBase
        {
            var obj = Spawn<T>(position, parent);
            obj.transform.rotation = rotation;
            return obj;
        }

        public T SpawnAudioSource<T>() where T : ObjectPoolBase
        {
            if (pools["AudioSource"].Count == 0)
            {
                var data = ObjectPoolDataSO.Instance.objectPoolDatas.Find(obj => obj.prefabName == "AudioSource");
                for (int i = 0; i < data.count; i++)
                {
                    var obj = Instantiate(data.prefab, data.parent);
                    obj.name.Replace("(Clone)", "");
                    pools["AudioSource"].Enqueue(obj);
                }
            }
            var retObj = (T)pools["AudioSource"].Dequeue();
            retObj.SetActive(true);
            return retObj;
        }

        public void Release(ObjectPoolBase item)
        {
            item.SetActive(false);
            var data = ObjectPoolDataSO.Instance.objectPoolDatas.Find(obj => obj.prefabName == item.name);
            item.transform.parent = data.parent;
            pools[item.name].Enqueue(item);
        }
    }
}