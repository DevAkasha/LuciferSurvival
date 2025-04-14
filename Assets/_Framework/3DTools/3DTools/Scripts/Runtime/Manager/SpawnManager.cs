using Ironcow.Data;
using Ironcow.ObjectPool;
using Ironcow.WorldObjectBase;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Ironcow.Tool3D
{
    public class SpawnManager : ManagerBase<SpawnManager>
    {
        [SerializeField] private bool is2D = false;
        [SerializeField] private Transform objectPool;
        private Transform ObjectPool
        {
            get
            {
                if (objectPool == null) objectPool = new GameObject("ObjectPool").transform;
                return objectPool;
            }
        }

        public override void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
        {
            isInit = true;
        }

        public bool OnRay(ref Vector3 position)
        {
            if (is2D)
            {
                position.z = 200;
            }
            else
            {
                position.y = 200;
            }
            Ray ray = new Ray(position, Vector3.down);
            if(is2D)
            {
                ray = new Ray(position, Vector3.back);
            }
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Ground")) return false;
                position = hit.point;
                return true;
            }
            return false;
        }

#if USE_SO_DATA
        public bool SpawnObject<D, T>(D data, Vector3 position, out T worldObject, bool isRandom = false) where D : BaseDataSO where T : WorldBase<D>
        {
            if (OnRay(ref position) || !isRandom)
            {
#if USE_OBJECT_POOL
                worldObject = PoolManager.instance.Spawn<T>(data.rcode, position, ObjectPool);
#else
                worldObject = (T)Instantiate(ResourceManager.instance.LoadAsset<WorldBase<D>>(data.rcode, ResourceType.Prefabs), position, Quaternion.identity, ObjectPool);
#endif
                worldObject.name = worldObject.name.Replace("(Clone)", "");
                worldObject.Init(data);
                return true;
            }
            worldObject = null;
            return false;
        }
#else
        public bool SpawnObject(BaseDataSO data, Vector3 position, bool isRandom = false)
        {
            if (OnRay(ref position) || !isRandom)
            {
#if USE_OBJECT_POOL
                var worldObject = PoolManager.instance.Spawn<WorldBase<BaseDataSO>>(data.rcode, position, ObjectPool);
#else
                var worldObject = Instantiate(ResourceManager.instance.LoadAsset<WorldBase<BaseDataSO>>(data.rcode, ResourceType.Prefabs), position, Quaternion.identity, ObjectPool);
#endif
                worldObject.name = worldObject.name.Replace("(Clone)", "");
                worldObject.Init(data);
                return true;
            }
            return false;
        }
#endif
    }
}