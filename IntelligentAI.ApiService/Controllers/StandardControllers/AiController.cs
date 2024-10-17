using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using IntelligentAI.ApiService.AspectInjectors;
using IntelligentAI.ApiService.Applications;

namespace IntelligentAI.ApiService.Controllers.StandardControllers;

[ApiController]
[Route("[controller]/[action]")]
[ApiExplorerSettings(GroupName = "Standard")]
[TypeFilter(typeof(ExceptionHandlerFilter))]
public class AiController : ControllerBase
{
    private readonly IAiClientFactory _modelFactory;
    private readonly IAiClientEventManager _eventManager;

    public AiController(IAiClientFactory modelFactory,IAiClientEventManager eventManager)
    {
        _eventManager = eventManager;
        _modelFactory = modelFactory;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="modelEnum"> Default: 6. Model name is qwen-long. </param>
    /// <param name="cancellation"></param>
    /// <returns></returns>    
    [HttpPost]
    [Log(MeasureTime = true)]
    public async Task<string> AnswerTextAsync(
        [FromBody] AiArguments request, 
        [FromQuery] int modelEnum = 6, 
        CancellationToken cancellation = default)
    {
        var model = _modelFactory.CreateClient(modelEnum);

        var result = await model.AnswerText(
            request.Question,
            request.ToDictionary(),
            request.Messages,
            cancellation: cancellation);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="modelEnum"> Default: 6. Model name is qwen-long. </param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    [HttpPost]
    public async IAsyncEnumerable<string> AnswerStringsAsync(
        [FromBody] AiArguments request,
        [FromQuery] int modelEnum = 6,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        var model = _modelFactory.CreateClient(modelEnum);

        await foreach (var result in model.AnswerStream(
            request.Question,
            request.ToDictionary(),
            request.Messages,
            cancellation: cancellation))
        {
            cancellation.ThrowIfCancellationRequested();

            // 必须保留，没有触发线程切换的情况下客户端无法做到单次抓取数据
            await Task.Delay(1);

            yield return result;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="modelEnum"> Default: 6. Model name is qwen-long. </param>
    /// <param name="streamType"> Default: false. ContentType is text/event-stream. Additional configuration for nginx is required. </param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AnswerStreamAsync(
        [FromBody] AiArguments request,
        [FromQuery] int modelEnum = 6,
        [FromQuery] bool streamType = false,
        CancellationToken cancellation = default)
    {
        Response.ContentType = streamType ? "application/octet-stream" : "text/event-stream";

        var model = _modelFactory.CreateClient(modelEnum);

        await foreach (var result in model.AnswerStream(
            request.Question,
            request.ToDictionary(),
            request.Messages,
            cancellation: cancellation))
        {
            cancellation.ThrowIfCancellationRequested();

            await Response.WriteAsync(result);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="modelEnum"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>    
    [HttpPost]
    public async Task<IActionResult> AnswerVideoAsync(
        [FromBody] AiVideoArguments request,
        [FromQuery] int modelEnum = 3,
        CancellationToken cancellation = default)
    {
        var model = _modelFactory.CreateClient(modelEnum);

        var message = await model.AnswerVideo(
            request.Question,
            request.ToDictionary(),
            cancellation: cancellation);

        return Ok(message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>    
    /// <param name="modelEnum"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>    
    [HttpPost]
    public async Task<IActionResult> AnswerImagesAsync(
        [FromBody] AiImagesArguments request,
        [FromQuery] int modelEnum = 3,
        CancellationToken cancellation = default)
    {
        var model = _modelFactory.CreateClient(modelEnum);

        var message = await model.AnswerVideo(
            request.Question,
            request.ToDictionary(),
            cancellation: cancellation);

        return Ok(message);
    }

    [HttpPost]
    public async IAsyncEnumerable<AiProgressResult> AnswerProgressAsync(
    [FromBody] List<AiArguments> requests,
    [FromQuery] int modelEnum = 6,
    [FromQuery] Guid? eventId = null,
    [FromQuery] Guid? parentTaskId = null,
    [FromQuery] string? taskName = null,
    [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        var model = _modelFactory.CreateClient(modelEnum);

        await foreach (var result in _eventManager.StartTasksAsync(
            model,
            eventId ?? Guid.NewGuid(),
            parentTaskId ?? Guid.NewGuid(),
            requests,
            taskName ?? "EventTasks",
            cancellation: cancellation))
        {
            cancellation.ThrowIfCancellationRequested(); 

            await Task.Delay(1);

            yield return result;
        }
    }

    /// <summary>
    /// 获取已实现的大模型Id 服务商名称 大模型Code
    /// </summary>
    /// <returns></returns>    
    [HttpGet]
    public IActionResult GetClients()
    {
        var modelInformations = ModelEnum.GetAll<ModelEnum>();

        return Ok(modelInformations);
    }

}
