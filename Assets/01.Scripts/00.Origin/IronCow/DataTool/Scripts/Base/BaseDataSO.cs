using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "BaseData", menuName = "ScriptableObjects/Base Data")]
public class BaseDataSO: ScriptableObject
{
    [SerializeField] public Sprite thumbnail;
    public string rcode;
    public string displayName;
    public string description;

    public object clone
    {
        get
        {
            var obj = MemberwiseClone();
            return obj;
        }
    }

    protected virtual void OnClone(object obj) { }
}
