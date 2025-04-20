using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

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
            var instance = (ICanvasOption)target;
            instance.SetParent();
        }
#if USE_AUTO_CACHING
        base.OnInspectorGUI();
#else
        DrawDefaultInspector();
#endif
    }
}