﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoAutoCaching : MonoBehaviour
{
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        AutoCaching();
    }

    public void AutoCaching()
    {
        var fields = this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (!field.FieldType.IsSubclassOf(typeof(Component)))
            {
                var value = field.GetValue(this);
                var isNull = value == null;
                if (!isNull) isNull = value.Equals(null);
                if (!isNull) continue;
                var components = GetComponentsInChildren(typeof(Transform), false);
                foreach (var component in components)
                {
                    if (component.name == field.Name) field.SetValue(this, component.gameObject);
                }
            }
            else
            {
                var value = field.GetValue(this);
                var isNull = value == null;
                if (!isNull) isNull = value.Equals(null);
                if (!isNull) continue;
                var components = GetComponentsInChildren(field.FieldType);
                foreach (var component in components)
                {
                    if (component.name == field.Name) field.SetValue(this, component);
                }
                isNull = value == null;
                if (!isNull) isNull = value.Equals(null);
                if (!isNull) continue;
                if (isNull && field.FieldType != typeof(Transform))
                {
                    if (gameObject.TryGetComponent(field.FieldType, out var component))
                        field.SetValue(this, component);
                }
            }
        }
    }

    public virtual void Release()
    {
        Destroy(gameObject);
    }
#endif
}
