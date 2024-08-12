using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using IntelligentAI.ApiService.Applications;

namespace IntelligentAI.ApiService.Controllers.StandardControllers;

[ApiController]
[Route("[controller]/[action]")]
[ApiExplorerSettings(GroupName = "Standard")]
[TypeFilter(typeof(ExceptionHandlerFilter))]
public class AiManagerController : ControllerBase
{
    private const string BusyMessage = "当前系统正忙，排队任务较多，请稍后再试...";

    private readonly IAiModelFactory _modelFactory;
    private readonly IAiModelEventManager _eventManager;

    public AiManagerController(IAiModelFactory modelFactory,IAiModelEventManager eventManager)
    {
        _eventManager = eventManager;
        _modelFactory = modelFactory;
    }

    [HttpPost]
    public async IAsyncEnumerable<AiProgressResult> AnswerProgressAsync(
        [FromBody]List<AiArguments> requests,
        [FromQuery] int modelEnum = 6, 
        [FromQuery] Guid? eventId = null,
        [FromQuery] Guid? parentTaskId = null,
        [FromQuery] string? taskName = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        if (await _eventManager.IsBusy(model, cancellation)) throw new ApplicationException(BusyMessage);

        await foreach (var result in _eventManager.StartTasksAsync(
            model,
            eventId ?? Guid.NewGuid(),
            parentTaskId ?? Guid.NewGuid(),
            requests,
            taskName ?? "EventTasks",
            cancellation: cancellation))
        {
            cancellation.ThrowIfCancellationRequested();

            yield return result;
        }
    }
}
