using Ironcow;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(KeyBindAttribute))]
public class KeyBindDrawer : PropertyDrawer
{
    bool isCheckKey = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        GUILayout.BeginHorizontal();
        var width = position.width;

        EditorGUI.LabelField(new Rect(position.x + 0, position.y, width / 3, position.height), label);
        EditorGUI.LabelField(new Rect(position.x + Mathf.Lerp(0, width, 0.33f), position.y, width / 3, position.height), property.enumDisplayNames[property.enumValueIndex]);
        var style = new GUIStyle(UnityEngine.GUI.skin.button);
        style.alignment = TextAnchor.MiddleCenter;
        GUIContent content = new GUIContent(isCheckKey ? "키 입력 대기중" : "키 바인딩");
        if(EditorGUI.DropdownButton(new Rect(position.x + Mathf.Lerp(0, width, 0.66f), position.y, width / 3, position.height), content, FocusType.Keyboard, style))
        {
            isCheckKey = true;
        }
        if (isCheckKey)
        {
            KeyCode targetKeyCode = KeyCode.None;
            if (Event.current.isKey)
            {
                targetKeyCode = Event.current.keyCode;
            }
            if (Event.current.isMouse)
            {
                targetKeyCode = Event.current.IsRightMouseButton() ? KeyCode.Mouse1 : KeyCode.Mouse0;
            }
            if(targetKeyCode > KeyCode.None)
            { 
                var instance = (KeyBindData)property.serializedObject.targetObject;
                var keyBind = instance.keyBind;
                var name = property.name.Split('<').Last().Split('>').First();
                var field = keyBind.GetType().GetField(name);
                if (field != null)
                {
                    field.SetValue(keyBind, (int)targetKeyCode);
                }
                else
                {
                    var pro = keyBind.GetType().GetProperty(name);
                    pro.SetValue(keyBind, (int)targetKeyCode);
                }
                //property.enumValueIndex = (int)Event.current.keyCode;
                instance.SetDirty();
                isCheckKey = false;
            }
        }
        GUILayout.EndHorizontal();
        //EditorGUI.PropertyField(position, property, label);
        EditorGUI.EndProperty();
    }
}