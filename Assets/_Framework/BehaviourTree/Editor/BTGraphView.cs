using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Ironcow.BT;
using System.Collections.Generic;

public class BTGraphView : GraphView
{
    public List<Edge> Edges => edges.ToList();

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Create/Selector", (a) => CreateNode(typeof(SelectorNode), GetLocalMousePosition(evt)));
        evt.menu.AppendAction("Create/Sequence", (a) => CreateNode(typeof(SequenceNode), GetLocalMousePosition(evt)));
        evt.menu.AppendAction("Create/Action", (a) => CreateNode(typeof(ActionNode), GetLocalMousePosition(evt)));
    }

    private Vector2 GetLocalMousePosition(ContextualMenuPopulateEvent evt)
    {
        return this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
    }

    public BTGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("BTGraphStyle"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(CreateEntryPointNode());
    }

    public BTNodeView CreateNode(Type type, Vector2 position)
    {
        BTNode node = Activator.CreateInstance(type) as BTNode;
        BTNodeView nodeView = new BTNodeView(node)
        {
            title = type.Name,
            GUID = Guid.NewGuid().ToString(),
            NodeType = type
        };
        nodeView.SetPosition(new Rect(position, new Vector2(150, 200)));
        AddElement(nodeView);
        return nodeView;
    }

    private BTNodeView CreateEntryPointNode()
    {
        var root = new RootNode();
        var nodeView = new BTNodeView(root)
        {
            title = "Root",
            GUID = "ROOT",
            NodeType = typeof(RootNode)
        };
        nodeView.SetPosition(new Rect(Vector2.zero, new Vector2(150, 100)));
        nodeView.capabilities &= ~Capabilities.Deletable;
        AddElement(nodeView);
        return nodeView;
    }

    public void ClearGraph()
    {
        // 기존 노드와 엣지 전부 제거
        graphElements.ForEach(RemoveElement);
    }
    public void Save(string fileName)
    {
        BTGraphSaveUtility.SaveGraph(this, fileName);
    }

    public void Load(string fileName)
    {
        BTGraphSaveUtility.LoadGraph(this, fileName);
    }

    public BTNodeView CreateNodeView(BTNode node, Vector2 position)
    {
        var nodeView = new BTNodeView(node)
        {
            GUID = System.Guid.NewGuid().ToString()
        };

        nodeView.SetPosition(new Rect(position, new Vector2(200, 150)));
        AddElement(nodeView);
        nodeView.Draw(); // 포트 및 내용 구성

        return nodeView;
    }

}
