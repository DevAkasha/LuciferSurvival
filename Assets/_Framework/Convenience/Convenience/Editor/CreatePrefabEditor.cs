using Ironcow;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreatePrefabEditor : Editor
{
    [MenuItem("GameObject/UI/Prefabs/Thumbnail", priority = -500)]
    private static void AddThumbnail()
    {
        var select = Selection.activeObject as GameObject;
        var path = Path.Combine(PrefabSetting.path, "thumbnail.prefab");
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
        go.transform.parent = select.transform;
        go.RectTransform().anchoredPosition = Vector2.zero;
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/UI/Prefabs/ScrollView Horizontal", priority = -500)]
    private static void AddScrollViewHorizontal()
    {
        var select = Selection.activeObject as GameObject;
        var path = Path.Combine(PrefabSetting.path, "ScrollViewHorizontal.prefab");
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
        go.transform.parent = select.transform;
        go.RectTransform().anchoredPosition = Vector2.zero;
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/UI/Prefabs/ScrollView Vertical", priority = -500)]
    private static void AddScrollViewVertical()
    {
        var select = Selection.activeObject as GameObject;
        var path = Path.Combine(PrefabSetting.path, "ScrollViewVertical.prefab");
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
        go.transform.parent = select.transform;
        go.RectTransform().anchoredPosition = Vector2.zero;
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/UI/Prefabs/PopupFrame", priority = -500)]
    private static void AddPopupFrame()
    {
        var select = Selection.activeObject as GameObject;
        var path = Path.Combine(PrefabSetting.path, "PopupFrame.prefab");
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
        go.transform.parent = select.transform;
        go.RectTransform().anchoredPosition = Vector2.zero;
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/UI/Prefabs/Currency", priority = -500)]
    private static void AddCurrency()
    {
        var select = Selection.activeObject as GameObject;
        var path = Path.Combine(PrefabSetting.path, "Currency.prefab");
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
        go.transform.parent = select.transform;
        go.RectTransform().anchoredPosition = Vector2.zero;
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Create UI Base", priority = -490)]
    private static void AddUIBase()
    {
        var select = Selection.activeObject as GameObject;
        var go = new GameObject(select.name == "Popup" ? "Popup" : "UI");
        go.transform.parent = select.transform;
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        Selection.activeObject = go;
    }

}