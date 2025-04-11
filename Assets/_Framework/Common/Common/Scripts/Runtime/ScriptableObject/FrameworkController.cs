using Ironcow;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ironcow
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class FrameworkController : SOSingleton<FrameworkController>
    {
        public bool isLocale;
        public bool isAutoCaching;
        public bool isScriptableObjectData;
        public bool isAddressableAsync;
        public bool isObjectPool;
        public bool isCloudCode;
        public bool isFSM;
    }
}