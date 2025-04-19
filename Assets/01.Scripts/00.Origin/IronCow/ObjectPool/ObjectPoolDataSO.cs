using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolData
{
    public string prefabName;
    public int count = 50;
    public int velocity = 25;
    [HideInInspector] public ObjectPoolBase prefab;
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