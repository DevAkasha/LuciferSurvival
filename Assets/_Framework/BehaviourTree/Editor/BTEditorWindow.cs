using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using System.IO;
using Ironcow.BT;
using UnityEngine.UIElements;

public class BTEditorWindow : EditorWindow
{
    private BTGraphView graphView;

    [MenuItem("Tools/BT Graph Editor")]
    public static void Open()
    {
        var window = GetWindow<BTEditorWindow>();
        window.titleContent = new GUIContent("BT Editor");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void ConstructGraphView()
    {
        graphView = new BTGraphView
        {
            name = "BT Graph"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new UnityEditor.UIElements.Toolbar();

        var popup = new UnityEditor.UIElements.ToolbarMenu();
        popup.text = "Create Node";
        popup.menu.AppendAction("Selector", a => graphView.CreateNode(typeof(SelectorNode), new Vector2(300, 300)));
        popup.menu.AppendAction("Sequence", a => graphView.CreateNode(typeof(SequenceNode), new Vector2(300, 300)));
        popup.menu.AppendAction("Action", a => graphView.CreateNode(typeof(ActionNode), new Vector2(300, 300)));
        toolbar.Add(popup);

        var saveBtn = new UnityEditor.UIElements.ToolbarButton(() => Save()) { text = "Save" };
        var loadBtn = new UnityEditor.UIElements.ToolbarButton(() => Load()) { text = "Load" };

        toolbar.Add(saveBtn);
        toolbar.Add(loadBtn);
        rootVisualElement.Add(toolbar);
    }

    private void Save()
    {
        graphView.Save("BTGraphStyle");
    }

    private void Load()
    {
        graphView.ClearGraph();
        graphView.Save("BTGraphStyle");
    }
}
