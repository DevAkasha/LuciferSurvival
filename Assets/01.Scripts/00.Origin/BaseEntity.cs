using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    protected virtual void Awake()
    {
        SetupModels();
    }

    protected abstract void SetupModels();
}
