using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Microsoft.AspNetCore.Components;

namespace IntelligentAI.Client.Components.Pages.WorkflowGroup;

public partial class DiagramDemo
{
    private BlazorDiagram Diagram { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Diagram = new BlazorDiagram();
        Diagram.RegisterComponent<AddTwoNumbersNode, AddTwoNumbersComponent>();

        Diagram.RegisterComponent<BotAnswerNode, BotAnswerWidget>();


        Setup();
    }

    private void Setup()
    {
        var node1 = NewNode(50, 50);
        var node2 = NewNode(300, 300);
        var node3 = NewNode(300, 50);
        var node = new AddTwoNumbersNode(new Blazor.Diagrams.Core.Geometry.Point(100, 50));
        var botNode = new BotAnswerNode(new Blazor.Diagrams.Core.Geometry.Point(50, 100));
        botNode.AddPort(PortAlignment.Top);
        botNode.AddPort(PortAlignment.Bottom);
        Diagram.Nodes.Add(botNode);


        Diagram.Nodes.Add(new[] { node1, node2, node3, node });
        Diagram.Links.Add(new LinkModel(node1.GetPort(PortAlignment.Right), node2.GetPort(PortAlignment.Left)));
    }

    private NodeModel NewNode(double x, double y)
    {
        var node = new NodeModel(new Blazor.Diagrams.Core.Geometry.Point(x, y));
        node.AddPort(PortAlignment.Bottom);
        node.AddPort(PortAlignment.Top);
        node.AddPort(PortAlignment.Left);
        node.AddPort(PortAlignment.Right);
        return node;
    }
}

