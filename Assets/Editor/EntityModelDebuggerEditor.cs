#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CustomEditor(typeof(BaseEntity), true)]
public class EntityModelDebuggerEditor : Editor
{
    private bool showRxDebug = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying) return;

        var entity = target as BaseEntity;
        if (entity == null) return;

        var model = entity.GetBaseModel();
        if (model == null) return;

        DrawModelDebugView(model);
    }

    private void DrawModelDebugView(BaseModel model)
    {
        EditorGUILayout.Space();
        showRxDebug = EditorGUILayout.Foldout(showRxDebug, "📊 RxModel Debug View", true);
        if (!showRxDebug) return;

        EditorGUI.indentLevel++;

        foreach (var rx in model.GetAllRxFields())
        {
            if (rx == null) continue;

            // RxStateFlagSet<T> 처리 (인터페이스 활용)
            if (rx is IRxInspectable inspectable)
            {
                inspectable.DrawDebugInspector();
                continue;
            }

            // RxStateFlag 단독 항목은 필드 출력에서 제외 (RxStateFlagSet 안에서만 표시됨)
            if (rx.GetType().Name.Contains("RxStateFlag") && !rx.GetType().IsGenericType)
            {
                continue;
            }

            // 일반 필드 출력
            if (rx is IRxField field)
            {
                string name = field.FieldName;
                string value = "(unknown)";

                if (rx is IRxModBase mod)
                    value = mod.Value?.ToString();
                else if (rx is IRxReadable<object> read)
                    value = read.Value?.ToString();

                GUIStyle style = new(EditorStyles.label);
                if (rx is IRxModBase) style.normal.textColor = Color.cyan;

                if (rx is IRxModFormulaProvider formulaProvider)
                {
                    string formula = formulaProvider.BuildDebugFormula();
                    GUIStyle combinedStyle = new(EditorStyles.label)
                    {
                        normal = { textColor = style.normal.textColor }
                    };
                    EditorGUILayout.LabelField(name, $"{value}   ←   {formula}", combinedStyle);
                }
                else
                {
                    EditorGUILayout.LabelField(name, value, style);
                }
            }
        }

        EditorGUI.indentLevel--;
    }
}
#endif
