using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolData
{
    public string prefabName;
    public int count = 50;
    //public int velocity = 25;
    [HideInInspector] public ObjectPoolBase prefab;
    [HideInInspector] public Transform parent;
}
[CreateAssetMenu(fileName = "ObjectPoolDataSO", menuName = "ScriptableObjects/ObjectPoolDataSO")]
public class ObjectPoolDataSO : SOSingleton<ObjectPoolDataSO>
{
    public void SaveSO()
    {
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
        SetDirty();
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
    }

    [SerializeField] public List<ObjectPoolData> objectPoolDatas = new List<ObjectPoolData>();
}