using IntelligentAI.Records;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace IntelligentAI.Abstraction;

public abstract class AiModelBase : ApiBase
{
    public string ServiceName { get; set; }

    public string ModelName { get; set; }

    public string ServiceKey => $"{ServiceName}-{ModelName}";

    public int ConcurrentNumber { get; set; }

    public string ApiKey { get; set; }


    #region 构造函数

    public AiModelBase(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {

    }

    public AiModelBase() : base()
    {

    }

    #endregion

    public abstract Task<string> AnswerText(
        string question,
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        CancellationToken cancellation = default);

    public abstract IAsyncEnumerable<string> AnswerStream(
        string question, 
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        CancellationToken cancellation = default);

    public abstract Task<string[]> AnswerImages(
        string input,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellation = default);

    public abstract Task<string> AnswerVideo(
        string input,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellation = default);

    protected abstract Dictionary<string, object> GetParameters(
        string method,
        Dictionary<string, object>? overrides = null,
        MissingKeyBehavior missingKeyBehavior = MissingKeyBehavior.Error);
    
}

