using UnityEditor;
using UnityEngine;

internal class UIEditor : Editor
{
    public static void CreateManagerInstance()
    {
        if (GameObject.Find("UIManager")) return;
        var obj = new GameObject("UIManager");
        obj.AddComponent<UIManager>();
    }
}
