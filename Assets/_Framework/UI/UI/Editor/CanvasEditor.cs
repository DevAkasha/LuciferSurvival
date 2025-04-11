using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using UnityEditor;
using Ironcow.UI;

[CustomEditor(typeof(CanvasBase), true)]
public class CavnasEditor :
#if USE_AUTO_CACHING
    MonoAutoCachingEditor
#else
    Editor
#endif
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Set Parents"))
        {
            var instance = (CanvasOption)target;
            instance.SetParent();
        }
#if USE_AUTO_CACHING
        base.OnInspectorGUI();
#else
        DrawDefaultInspector();
#endif
    }
}