using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(CheckAttribute))]
public class CheckDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);

        var inputType = (eInputType)property.enumValueIndex;
        var obj = (KeyBindData)property.serializedObject.targetObject;
        var instance = property.serializedObject.targetObject as KeyBindData;
        switch (inputType)
        {
            case eInputType.Press:
                {
                    obj.keyBind = instance.keyBindPress;
                }
                break;
            case eInputType.AxisY:
            case eInputType.AxisX:
                {
                    obj.keyBind = instance.keyBindAxis;
                }
                break;
        }
    }
}