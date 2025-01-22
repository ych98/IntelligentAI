using IntelligentAI.Models;

namespace IntelligentAI.Abstraction;

public interface IAiClientEventManager
{
    IAsyncEnumerable<AiProgressResult> StartTasksAsync(
        AiClientBase model,
        Guid eventId,
        Guid parentTaskId,
        IEnumerable<AiArguments> tasks,
        string taskName = "EventTasks",
        CancellationToken cancellation = default);
}

