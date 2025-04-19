using Ironcow;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ironcow.ObjectPool
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class ObjectPoolPathSetting : SOSingleton<ObjectPoolPathSetting>
    {
#if UNITY_EDITOR
        public Object objectPoolPath;
        public static string ObjectPoolPath { get => AssetDatabase.GetAssetPath(Instance.objectPoolPath); }
#endif
    }
}