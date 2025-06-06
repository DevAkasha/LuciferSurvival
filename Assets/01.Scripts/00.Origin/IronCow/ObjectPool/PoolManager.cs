using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private List<ObjectPoolData> sheets;
    Dictionary<string, Queue<ObjectPoolBase>> pools = new Dictionary<string, Queue<ObjectPoolBase>>();
    public bool isInit = false;
    private GameObject PoolParent;

    public void Init(string resourceType, bool clear = false)
    {
        if (clear)
        {
            sheets.Clear();
            pools.Clear();
        }
           
        InitializePoolData(resourceType);

        if (PoolParent == null)
            PoolParent = GameObject.Find("PoolParent");

        foreach (var data in sheets)
        {
            string parentName = data.prefabName + "parent";
            if (PoolParent.transform.Find(parentName) != null)
            {
                continue;
            }

            data.prefab = ResourceManager.Instance.LoadAsset<ObjectPoolBase>(data.prefabName, resourceType);//Resources폴더의 바로 하위에 가져올 폴더를 생성해야한다.
            data.parent = new GameObject(data.prefabName + "parent").transform;
            data.parent.position = transform.position;
            data.parent.parent = PoolParent.transform;

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
            var data = sheets.Find(obj => obj.prefabName == rcode);
            for (int i = 0; i < data.count; i++)
            {
                var obj = Instantiate(data.prefab, data.parent);
                obj.name = obj.name.Replace("(Clone)", "");
                obj.SetActive(false);
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

    public T Spawn<T>(string rcode, Vector3 position) where T : ObjectPoolBase
    {
        var obj = Spawn<T>(rcode);
        obj.transform.position = position;
        return obj;
    }

    public T SpawnAudioSource<T>(string audioName, Vector3 position) where T : ObjectPoolBase
    {
        if (pools[audioName].Count == 0)
        {
            var data = sheets.Find(obj => obj.prefabName == audioName);
            for (int i = 0; i < data.count; i++)
            {
                var obj = Instantiate(data.prefab, data.parent);
                obj.name.Replace("(Clone)", "");
                pools[audioName].Enqueue(obj);
            }
        }
        var retObj = (T)pools[audioName].Dequeue();
        retObj.transform.position = position;
        retObj.SetActive(true);
        return retObj;
    }

    public void Release(ObjectPoolBase item)
    {
        item.SetActive(false);
        var data = sheets.Find(obj => obj.prefabName == item.name);
        item.transform.parent = data.parent;
        item.transform.position = transform.position;
        pools[item.name].Enqueue(item);
    }

    public void InitializePoolData(string resourceType)
    {
        //sheets.Clear();

        // Resources/resourceType 폴더에서 모든 프리팹을 불러오기
        var prefabs = Resources.LoadAll<GameObject>(resourceType);

        foreach (var prefab in prefabs)
        {
            var poolComponent = prefab.GetComponent<ObjectPoolBase>();
            if (poolComponent == null)
            {
                continue;
            }

            ObjectPoolData data = new ObjectPoolData
            {
                prefabName = prefab.name,
                count = 1
            };

            sheets.Add(data);
        }
    }


}