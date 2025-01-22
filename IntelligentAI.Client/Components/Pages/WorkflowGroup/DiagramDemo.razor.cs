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
        
        Diagram.RegisterComponent<AddTwoNumbersNode, AddTwoNumbersWidget>();

        Diagram.RegisterComponent<BotAnswerNode, BotAnswerWidget>();

        Setup();
    }

    private void Setup()
    {
        var node1 = NewNode(50, 50);

        var node = new AddTwoNumbersNode(new Blazor.Diagrams.Core.Geometry.Point(300, 100));
        node.AddPort(PortAlignment.Top);
        node.AddPort(PortAlignment.Bottom);

        var botNode = new BotAnswerNode(new Blazor.Diagrams.Core.Geometry.Point(500, 300));
        botNode.AddPort(PortAlignment.Top);
        botNode.AddPort(PortAlignment.Bottom);

        Diagram.Nodes.Add(botNode);

        Diagram.Nodes.Add(new[] { node1, node });
        Diagram.Links.Add(new LinkModel(botNode.GetPort(PortAlignment.Top), node1.GetPort(PortAlignment.Right)));

        node.Title = botNode.Answer;

    }

    private NodeModel NewNode(double x, double y)
    {
        var node = new NodeModel(new Blazor.Diagrams.Core.Geometry.Point(x, y));
        node.AddPort(PortAlignment.Left);
        node.AddPort(PortAlignment.Right);
        return node;
    }
}

