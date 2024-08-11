using Blazor.Diagrams;

namespace IntelligentAI.Components.Pages.WorkflowGroup;

public partial class RestApi
{
    private BlazorDiagram Diagram { get; set; } = null!;

    protected override void OnInitialized()
    {
        Diagram = new BlazorDiagram();
    }
}
