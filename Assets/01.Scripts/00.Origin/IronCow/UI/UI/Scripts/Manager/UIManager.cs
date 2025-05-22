using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public static T Show<T>(T prefab, Transform parent, params object[] param) where T : UIBase
    {
        GameObject go = Instantiate(prefab.gameObject, parent);
        go.SetActive(true);
        var ui = go.GetComponent<T>();
        ui.opened?.Invoke(param);
        return ui;
    }

    public static void Hide(UIBase ui, params object[] param)
    {
        Destroy(ui.gameObject);
        ui.closed?.Invoke(param);
    }
}
