using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPoolBase : MonoBehaviour
{
    public abstract void Init(params object[] param);

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    void Release()
    {
        PoolManager.Instance.Release(this);
    }
}
