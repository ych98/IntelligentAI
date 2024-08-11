using Blazor.Diagrams;
using Blazor.Diagrams.Core.Models;

namespace IntelligentAI.Client.Components.Pages.WorkflowGroup;

public partial class DragAndDrop
{
    private readonly BlazorDiagram _blazorDiagram = new BlazorDiagram();
    private int? _draggedType;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _blazorDiagram.RegisterComponent<BotAnswerNode, BotAnswerWidget>(); 
        
        var node = new BotAnswerNode(new Blazor.Diagrams.Core.Geometry.Point(500, 500));
        node.AddPort(PortAlignment.Top);
        node.AddPort(PortAlignment.Bottom);
        _blazorDiagram.Nodes.Add(node);
    }

    private void OnDragStart(int key)
    {
        // Can also use transferData, but this is probably "faster"
        _draggedType = key;
    }

    private void OnDrop(Microsoft.AspNetCore.Components.Web.DragEventArgs e)
    {
        if (_draggedType == null) // Unkown item
            return;

        var position = _blazorDiagram.GetRelativeMousePoint(e.ClientX, e.ClientY);
        var node = _draggedType == 0 ? new NodeModel(position) : new BotAnswerNode(position);
        node.AddPort(PortAlignment.Top);
        node.AddPort(PortAlignment.Bottom);
        _blazorDiagram.Nodes.Add(node);
        _draggedType = null;
    }
}

public class BotAnswerNode : NodeModel
{
    public BotAnswerNode(Blazor.Diagrams.Core.Geometry.Point position = null) : base(position) { }

    public string Answer { get; set; }
}
