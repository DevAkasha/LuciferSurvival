using Ironcow;
using Ironcow.Convenience;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Ironcow.ObjectPool
{
    [System.Serializable]
    public class ObjectPoolData
    {
        public string prefabName;
        public int count = 50;
        public int velocity = 25;
        [HideInInspector] public IObjectPoolBase prefab;
        [HideInInspector] public Transform parent;
    }

    public class ObjectPoolDataSO : SOSingleton<ObjectPoolDataSO>
    {
        public void SaveSO()
        {
            SetDirty();
        }

        [SerializeField] public List<ObjectPoolData> objectPoolDatas = new List<ObjectPoolData>();
    }
}