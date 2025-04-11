using Ironcow;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;
using static UnityEngine.UI.Button;

public class ClassMethodPair
{
    public IDelegate instance;
    public MethodInfo method;
}

[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    int selectedIdx = -1;
    InputManager instance => (InputManager)this.target;
    public override void OnInspectorGUI()
    {
        if (selectedIdx == -1)
        {
            selectedIdx = instance.keyBindDatas.IndexOf(instance.keyBindDatas.FindAll(obj => obj.inputType == eInputType.Press).First());
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("IsDontDestroy");
        instance.isDontDestroy = GUILayout.Toggle(instance.isDontDestroy, "");
        GUILayout.EndHorizontal();
        var selectedData = instance.keyBindDatas[selectedIdx];
        var content = new GUIContent(selectedData.name);
        if (EditorGUILayout.DropdownButton(content, FocusType.Passive))
        {
            GenericMenu menu = new GenericMenu();
            int idx = 0;
            string selectedEnumDropdown = selectedData.name;
            foreach (var data in instance.keyBindDatas)
            {
                if (data.inputType != eInputType.Press) continue;
                var isEquip = data.name == selectedEnumDropdown;
                menu.AddItem(new GUIContent(data.name), isEquip, (dt) =>
                {
                    selectedIdx = instance.keyBindDatas.IndexOf((KeyBindData)dt);
                }, data);
                idx++;
            }
            menu.ShowAsContext();
        }

        var styleLeft = new GUIStyle(UnityEngine.GUI.skin.label);
        styleLeft.alignment = TextAnchor.MiddleLeft;
        var styleRight = new GUIStyle(UnityEngine.GUI.skin.label);
        styleRight.alignment = TextAnchor.MiddleRight;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Key", styleLeft);
        GUILayout.Label(selectedData.key, styleRight);
        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Key Code", styleLeft);
        GUILayout.Label(selectedData.keyBind.KeyCode.ToString(), styleRight);
        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        selectedData.SetEventPair(selectedData.key);
        SetList("On Key Down", selectedData.keyDownList, selectedData.OnKeyDown.GetType());
        SetList("On Key", selectedData.keyList, selectedData.OnKey.GetType());
        SetList("On Key Up", selectedData.keyUpList, selectedData.OnKeyUp.GetType());
    }
    
    public void SetList(string label, List<EventPair> list, Type type)
    {
        var selectedData = instance.keyBindDatas[selectedIdx];
        ReorderableList rlist = new ReorderableList(list, type, true, true, true, true);
        rlist.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, label.ToUpper());
        };
        rlist.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            rect.height -= 4;
            rect.y += 2;
            if (list[index].obj == null) list[index].obj = GameObject.Find(list[index].objName);
            list[index].obj = EditorGUI.ObjectField(new Rect(rect.position.x, rect.position.y, rect.width / 3, rect.height), list[index].obj, typeof(GameObject), true);
            var content = new GUIContent(string.IsNullOrEmpty(list[index].method) ? "<none>" : list[index].method);
            if (EditorGUI.DropdownButton(new Rect(rect.position.x + rect.width / 3 + 5, rect.position.y, rect.width / 3 * 2 - 5, rect.height), content, FocusType.Passive))
            {
                var monos = list[index].obj.GetComponents<IDelegate>();
                GenericMenu menuClass = new GenericMenu();
                string selectedEnumDropdown = list[index].method;
                List<ClassMethodPair> cmList = new List<ClassMethodPair>();
                foreach(var mono in monos)
                {
                    var methods = mono.GetType().GetDeclaredMethods();
                    foreach (var method in methods)
                    {
                        if (method.GetParameters().Length != 0) continue;
                        if (method.Name.Contains("get_")) continue;
                        if (method.ReturnType == typeof(IEnumerator)) continue;
                        cmList.Add(new ClassMethodPair() { instance = mono, method = method });
                    }
                }
                foreach (var data in cmList)
                {
                    var isEquip = data.instance.GetType().ToString() + "." + data.method.Name == selectedEnumDropdown;
                    menuClass.AddItem(new GUIContent(data.instance.GetType().ToString() + "." + data.method.Name), isEquip, (dt) =>
                    {
                        list[index].method = data.instance.GetType().ToString() + "." + (string)dt;
                        list[index].objName = list[index].obj.name;
                        var action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), data.instance, data.method);
                        if (selectedData.OnKeyDown == null) selectedData.OnKeyDown = new ButtonClickedEvent();
                        selectedData.OnKeyDown.AddListener(action);

                    }, data.method.Name);
                }
                menuClass.ShowAsContext();
            }
        };
        rlist.onAddCallback = lt =>
        {
            list.Add(new EventPair());
        };
        rlist.DoLayoutList();
    }

}
