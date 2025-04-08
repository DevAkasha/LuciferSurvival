using Ironcow;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;

public enum eInputType
{
    Press,
    AxisX,
    AxisY,
}

public enum eAxisDirection
{
    Left,
    Right,
    Up,
    Down,
}

public interface IDelegate
{

}

public class EventPair
{
    public UnityEngine.Object obj;
    public string objName;
    public string method;
}


[CreateAssetMenu(fileName = "KeyBindData", menuName = "ScriptableObjects/KeyBindData")]
public class KeyBindData : ScriptableObject
{
    public string key;
    public string name;
    [Check] public eInputType inputType;

    [SerializeReference] public KeyBindBase keyBind = new KeyBindPress();

    [HideInInspector] public ButtonClickedEvent OnKeyDown;
    [HideInInspector] public ButtonClickedEvent OnKey;
    [HideInInspector] public ButtonClickedEvent OnKeyUp;

    public float axis
    { 
        get
        {
            return Input.GetKey(keyBind.decrease) ? -1 : Input.GetKey(keyBind.increase) ? 1 : 0;
        }
    }

    public bool isButtonDown { get => Input.GetKeyDown(keyBind.KeyCode); }
    public bool isButtonPress { get => Input.GetKey(keyBind.KeyCode); }
    public bool isButtonUp { get => Input.GetKeyUp(keyBind.KeyCode); }

    public List<EventPair> keyDownList;
    public List<EventPair> keyList;
    public List<EventPair> keyUpList;

    [HideInInspector] public KeyBindPress keyBindPress; 
    [HideInInspector] public KeyBindAxis keyBindAxis;

    public void SetEventPair(string key)
    {
        if (keyDownList == null)
        {
            keyDownList = new List<EventPair>();
            for (int i = 0; i < OnKeyDown.GetPersistentEventCount(); i++)
            {
                EventPair pair = new EventPair();
                pair.obj = OnKeyDown.GetPersistentTarget(i);
                pair.method = OnKeyDown.GetPersistentMethodName(i);
                keyDownList.Add(pair);
            }
        }
        if (keyList == null)
        {
            keyList = new List<EventPair>();
            for (int i = 0; i < OnKey.GetPersistentEventCount(); i++)
            {
                EventPair pair = new EventPair();
                pair.obj = OnKey.GetPersistentTarget(i);
                pair.method = OnKey.GetPersistentMethodName(i);
                keyList.Add(pair);
            }
        }
        if (keyUpList == null)
        {
            keyUpList = new List<EventPair>();
            for (int i = 0; i < OnKeyUp.GetPersistentEventCount(); i++)
            {
                EventPair pair = new EventPair();
                pair.obj = OnKeyUp.GetPersistentTarget(i);
                pair.method = OnKeyUp.GetPersistentMethodName(i);
                keyUpList.Add(pair);
            }
        }
    }
}
