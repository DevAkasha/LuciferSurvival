using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Ironcow.BT;

public class BTNodeView : Node
{
    public string GUID;
    public BTNode node;
    public Type NodeType;

    public Port input;
    public Port output;

    public BTNodeView(BTNode node)
    {
        this.node = node;
        this.title = node.GetType().Name;
        this.GUID = Guid.NewGuid().ToString();

        CreateInputPort();
        CreateOutputPort();

        RefreshExpandedState();
        RefreshPorts();
    }

    void CreateInputPort()
    {
        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        input.portName = "";
        inputContainer.Add(input);
    }

    void CreateOutputPort()
    {
        // 액션 노드는 자식이 없으므로 출력 포트 생략
        if (!(node is ActionNode))
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            output.portName = "";
            outputContainer.Add(output);
        }
    }


    public void Draw()
    {
        // 기본 입출력 포트 설정
        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        input.portName = "";
        input.name = "Input";
        inputContainer.Add(input);

        output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        output.portName = "";
        output.name = "Output";
        outputContainer.Add(output);

        RefreshExpandedState();
        RefreshPorts();
    }
}
