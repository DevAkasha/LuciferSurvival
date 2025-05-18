
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UIPosition
{
    UI,
    Popup,
    GNB,
    Top,
}

public class UIManager1 : ManagerBase<UIManager1>
{
    [SerializeField] private List<Transform> parents;
    [SerializeField] private Transform worldParent;

    private List<UIBase1> UIList = new List<UIBase1>();

    public override void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
    {
        IsInit = true;
    }

    public static void SetWorldCanvas(Transform worldCanvas)
    {
        Instance.worldParent = worldCanvas;
    }

    public static void SetParents(List<Transform> parents)
    {
        Instance.parents = parents;
        Instance.UIList.Clear();
    }

    public static T Show<T>(params object[] param) where T : UIBase1
    {
        var key = typeof(T).ToString();
        var ui = Instance.UIList.FindLast(obj => obj.name == key);
        if (ui == null || ui.uiOptions.isMultiple)
        {
            var prefab = ResourceManager.Instance.LoadAsset<T>(key, ResourceType.UI);
            ui = Instantiate(prefab, Instance.parents[(int)prefab.uiPosition]);
            ui.name = key;
            Instance.UIList.Add(ui);
        }
        if (ui.uiPosition == UIPosition.UI && ui.uiOptions.isActiveOnLoad)
        {
            Instance.UIList.ForEach(obj =>
            {
                if (obj.uiPosition == UIPosition.UI) obj.gameObject.SetActive(false);
            });
        }
        ui.SetActive(ui.uiOptions.isActiveOnLoad);
        ui.opened?.Invoke(param);
        ui.uiOptions.isActiveOnLoad = true;
        return (T)ui;
    }

    public static void Hide<T>(params object[] param) where T : UIBase1
    {
        var key = typeof(T).ToString();
        var ui = Instance.UIList.FindLast(obj => obj.name == key);
        if (ui != null)
        {
            Instance.UIList.Remove(ui);
            if (ui.uiPosition == UIPosition.UI)
            {
                var prevUI = Instance.UIList.FindLast(obj => obj.uiPosition == UIPosition.UI);
                prevUI.SetActive(true);
            }
            ui.closed?.Invoke(param);
            if (ui.uiOptions.isDestroyOnHide)
            {
                Destroy(ui.gameObject);
            }
            else
            {
                ui.SetActive(false);
            }
        }
    }


    public static T Get<T>() where T : UIBase1
    {
        var key = typeof(T).ToString();
        return (T)Instance.UIList.Find(obj => obj.name == key);
    }

    public static bool IsOpened<T>() where T : UIBase1
    {
        var key = typeof(T).ToString();
        var ui = Instance.UIList.Find(obj => obj.name == key);
        return ui != null && ui.gameObject.activeInHierarchy;
    }

    public static void ShowIndicator()
    {

    }

    public static void HideIndicator()
    {

    }
    public static void ShowAlert(string desc, string title = "", string okBtn = "OK", string cancelBtn = "Cancel", UnityAction okCallback = null, UnityAction cancelCallback = null)
    {
        Show<PopupAlert>(desc, title, okBtn, cancelBtn, okCallback, cancelCallback);
    }


    public static void ShowAlert<T>(string desc, string title = "", string okBtn = "OK", string cancelBtn = "Cancel", UnityAction okCallback = null, UnityAction cancelCallback = null, T image = default)
    {
        Show<PopupAlert>(desc, title, okBtn, cancelBtn, okCallback, cancelCallback, image);
    }

    public static void ShowInputAlert(string desc, string title = "", UnityAction<string> okCallback = null, UnityAction cancelCallback = null, string okBtn = "", string cancelBtn = "")
    {
        Show<PopupAlert>(desc, title, okBtn, cancelBtn, okCallback, cancelCallback);
    }



    public static void HideAlert()
    {
        Hide<PopupAlert>();
    }
}

