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
    private readonly IAiModelFactory _modelFactory;
    private readonly IAiModelEventManager _eventManager;

    private const string BusyMessage = "当前系统正忙，排队任务较多，请稍后再试...";

    public AiController(IAiModelFactory modelFactory,IAiModelEventManager eventManager)
    {
        _eventManager = eventManager;
        _modelFactory = modelFactory;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="score">返回答案和content的相似分数，0：不返回分数，1：返回分数</param>
    /// <param name="modelEnum"> Default: 6. Model name is qwen-long. </param>
    /// <param name="cancellation"></param>
    /// <returns></returns>    
    [HttpPost]
    [Log(MeasureTime = true)]
    public async Task<string> AnswerTextAsync(
        [FromBody] AiArguments request, 
        [FromQuery] int score = 0,
        [FromQuery] int modelEnum = 6, 
        CancellationToken cancellation = default)
    {

        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        if (await _eventManager.IsBusy(model, cancellation)) throw new ApplicationException(BusyMessage);

        Dictionary<string, object> parameters = request.ToDictionary();
        parameters.Add("score", score);

        var result = await model.AnswerText(
            request.Question, 
            parameters,
            request.Messages,
            cancellation: cancellation);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="score">和question匹配的文档相关度阀值，低于这个数的采用</param>
    /// <param name="apiVersion">2为新版流式接口，进行了幻觉问题优化</param>
    /// <param name="modelEnum"> Default: 6. Model name is qwen-long. </param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    [HttpPost]
    public async IAsyncEnumerable<string> AnswerStringsAsync(
        [FromBody] AiArguments request, 
        [FromQuery] double score = 0.6, 
        [FromQuery] int apiVersion = 2, 
        [FromQuery] int modelEnum = 6,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        if (await _eventManager.IsBusy(model, cancellation)) throw new ApplicationException(BusyMessage);

        Dictionary<string, object> parameters = request.ToDictionary();
        parameters.Add("score", score);
        parameters.Add("apiVersion", apiVersion);

        await foreach (var result in model.AnswerStream(
            request.Question,
            parameters,
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
    /// <param name="score"></param>
    /// <param name="apiVersion"> </param>
    /// <param name="modelEnum"> Default: 6. Model name is qwen-long. </param>
    /// <param name="streamType"> Default: false. ContentType is text/event-stream. Additional configuration for nginx is required. </param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task AnswerStreamAsync(
        [FromBody] AiArguments request,
        [FromQuery] double score = 0.6,
        [FromQuery] int apiVersion = 2,
        [FromQuery] int modelEnum = 6,
        [FromQuery] bool streamType = false,
        CancellationToken cancellation = default)
    {
        Response.ContentType = streamType ? "application/octet-stream" : "text/event-stream";

        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        if (await _eventManager.IsBusy(model, cancellation)) throw new ApplicationException(BusyMessage);

        Dictionary<string, object> parameters = request.ToDictionary();
        parameters.Add("score", score);
        parameters.Add("apiVersion", apiVersion);

        await foreach (var result in model.AnswerStream(
            request.Question,
            parameters,
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
        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        Dictionary<string, object> parameters = request.ToDictionary();

        var message = await model.AnswerVideo(
            request.Question,
            parameters,
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
        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        Dictionary<string, object> parameters = request.ToDictionary();

        var message = await model.AnswerVideo(
            request.Question,
            parameters,
            cancellation: cancellation);

        return Ok(message);
    }

    /// <summary>
    /// 获取已实现的大模型Id 服务商名称 大模型Code
    /// </summary>
    /// <returns></returns>    
    [HttpGet]
    public IActionResult AnswerModels()
    {
        var modelInformations = ModelEnum.GetAll<ModelEnum>().Where(x => x.Id > 1);

        return Ok(modelInformations);
    }

}
