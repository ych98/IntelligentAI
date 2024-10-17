using IntelligentAI.Records;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace IntelligentAI.Abstraction;

public abstract class AiClientBase(HttpClient httpClient) : ApiClientBase(httpClient)
{
    public required string ServiceName { get; set; }

    public required string ModelName { get; set; }

    public string ServiceKey => $"{ServiceName}-{ModelName}";

    public string? ApiKey { get; set; }

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

public class AIProviderSettings
{
    public required string Host { get; set; }

    public required string ApiKey { get; set; }

    public required List<string> Models { get; set; }
}
