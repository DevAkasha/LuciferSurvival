using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.Events;


public class InputManager : MonoSingleton<InputManager>
{
    [SerializeField] public List<KeyBindData> keyBindDatas => KeyBindListData.Instance.datas;

    private Vector3 axis;
    public Vector3 Axis { get => this.axis.normalized; }

    private UnityAction<Vector3> axisAction;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < keyBindDatas.Count; i++)
        {
            if (keyBindDatas[i].inputType == eInputType.Press)
                StartCoroutine(ButtonCheck(i, keyBindDatas[i]));
            else
                StartCoroutine(CheckAxis(keyBindDatas[i]));
        }
        StartCoroutine(CheckJoyStick());
    }

    public IEnumerator ButtonCheck(int index, KeyBindData inputSystem)
    {
        var down = new WaitUntil(() => inputSystem.isButtonDown);
        var up = new WaitUntil(() => inputSystem.isButtonUp);
        while (true)
        {
            yield return down;
            inputSystem.OnKeyDown?.Invoke();
            while (!inputSystem.isButtonUp)
            {
                inputSystem.OnKey?.Invoke();
                yield return null;
            }
            inputSystem.OnKeyUp?.Invoke();
            yield return up;
        }
    }

    public IEnumerator CheckAxis(KeyBindData inputSystem)
    {
        while (true)
        {
            if (inputSystem.inputType == eInputType.AxisX)
            {
                this.axis.x = inputSystem.axis;
            }
            if (inputSystem.inputType == eInputType.AxisY)
            {
                this.axis.y = inputSystem.axis;
            }
            axisAction?.Invoke(this.axis.normalized);
            yield return null;
        }
    }

    public IEnumerator CheckJoyStick()
    {
        var joystick = Ironcow.JoyStick.VirtualStick.instance;
        if (joystick == null) yield break;
        else
        {
            while (true)
            {
                if (joystick.inputVector != Vector2.zero)
                {
                    axis = joystick.inputVector;
                }
                axisAction?.Invoke(this.axis.normalized);
                yield return null;
            }
        }
    }

    public void Subscribe(string key, UnityAction OnKeyDown = null, UnityAction OnKey = null, UnityAction OnKeyUp = null)
    {
        var index = keyBindDatas.FindIndex(obj => obj.key == key);
        keyBindDatas[index].OnKeyDown.AddListener(OnKeyDown);
        keyBindDatas[index].OnKey.AddListener(OnKey);
        keyBindDatas[index].OnKeyUp.AddListener(OnKeyUp);
    }

    public void SubscribeAxis(UnityAction<Vector3> action)
    {
        this.axisAction = action;
    }

}