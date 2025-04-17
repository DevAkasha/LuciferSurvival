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
        // �׼� ���� �ڽ��� �����Ƿ� ��� ��Ʈ ����
        if (!(node is ActionNode))
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            output.portName = "";
            outputContainer.Add(output);
        }
    }


    public void Draw()
    {
        // �⺻ ����� ��Ʈ ����
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
