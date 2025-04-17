using Ironcow.BT;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BTGraphSaveUtility
{
    public static void SaveGraph(BTGraphView graphView, string fileName)
    {
        var container = ScriptableObject.CreateInstance<BTGraphContainer>();
        container.Nodes.Clear();
        container.Links.Clear();

        foreach (var node in graphView.nodes)
        {
            if (node is BTNodeView nodeView)
            {
                container.Nodes.Add(new NodeSaveData
                {
                    GUID = nodeView.GUID,
                    Type = nodeView.node.GetType().FullName,
                    Position = nodeView.GetPosition().position
                });
            }
        }

        foreach (var edge in graphView.edges)
        {
            if (edge.input?.node is BTNodeView inputNode && edge.output?.node is BTNodeView outputNode)
            {
                container.Links.Add(new LinkSaveData
                {
                    OutputNodeGUID = outputNode.GUID,
                    InputNodeGUID = inputNode.GUID
                });
            }
        }

        string path = $"Assets/BTSaveData/{fileName}.asset";
        if (!Directory.Exists("Assets/BTSaveData"))
        {
            Directory.CreateDirectory("Assets/BTSaveData");
        }

        AssetDatabase.CreateAsset(container, path);
        AssetDatabase.SaveAssets();
    }

    public static void LoadGraph(BTGraphView graphView, string fileName)
    {
        string path = $"Assets/BTSaveData/{fileName}.asset";
        var container = AssetDatabase.LoadAssetAtPath<BTGraphContainer>(path);

        if (container == null)
        {
            Debug.LogError("저장된 그래프를 찾을 수 없습니다.");
            return;
        }

        graphView.ClearGraph();

        Dictionary<string, BTNodeView> nodeMap = new();

        // 노드 복원
        foreach (var nodeData in container.Nodes)
        {
            var type = System.Type.GetType(nodeData.Type);
            var node = System.Activator.CreateInstance(type) as BTNode;
            var nodeView = graphView.CreateNodeView(node, nodeData.Position);
            nodeView.GUID = nodeData.GUID;

            nodeMap.Add(nodeView.GUID, nodeView);
        }

        // 연결 복원
        foreach (var link in container.Links)
        {
            if (nodeMap.TryGetValue(link.OutputNodeGUID, out var outputNode) &&
                nodeMap.TryGetValue(link.InputNodeGUID, out var inputNode))
            {
                var edge = outputNode.output.ConnectTo(inputNode.input);
                graphView.AddElement(edge);
            }
        }
    }
}

[System.Serializable]
public class NodeSaveData
{
    public string GUID;
    public string Type;
    public Vector2 Position;
}

[System.Serializable]
public class LinkSaveData
{
    public string OutputNodeGUID;
    public string InputNodeGUID;
}

[System.Serializable]
public class BTGraphContainer : ScriptableObject
{
    public List<NodeSaveData> Nodes = new();
    public List<LinkSaveData> Links = new();
}