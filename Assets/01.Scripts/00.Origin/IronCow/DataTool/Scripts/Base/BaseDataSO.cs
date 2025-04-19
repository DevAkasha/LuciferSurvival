using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "BaseData", menuName = "ScriptableObjects/Base Data")]
public class BaseDataSO
#if USE_SO_DATA
: ScriptableObject
#endif
{
#if USE_SO_DATA
    [SerializeField] public Sprite thumbnail;
#endif
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
