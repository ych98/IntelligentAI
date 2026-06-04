using FluentHttp.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace IntelligentAI.Abstraction;

public class ApiClientBase(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Api Post Form 调用
    /// </summary>
    protected virtual async Task<TOut> CallApiAsync<TOut>(
        string url,
        Dictionary<string, object> args,
        CancellationToken cancellation = default)
    {
        return await httpClient.PostFromFormAsync<TOut>(
            url,
            args,
            cancellation);
    }

    /// <summary>
    /// 流式 Api Post Form 调用
    /// </summary>
    protected virtual async IAsyncEnumerable<TOut> CallStreamApiAsync<TOut>(
        string url,
        Dictionary<string, object> args,
        [EnumeratorCancellation] CancellationToken cancellation)
    {
        await foreach (var line in httpClient.PostStreamAsync<string>(
            url,
            args,
            FluentHttpExtensions.EnumerableStream,
            cancellation))
        {
            yield return DeserializeStreamLine<TOut>(line);
        }
    }

    /// <summary>
    /// Api Get Json 调用
    /// </summary>
    protected virtual async Task<TOut> GetAsync<TOut>(
        string url,
        string? bearer = null,
        CancellationToken cancellation = default)
    {
        PrepareClient(bearer);

        return await httpClient.GetFromJsonAsync<TOut>(
            url,
            cancellation);
    }

    /// <summary>
    /// Api Post Json 调用
    /// </summary>
    protected virtual async Task CallAsync(
        string url,
        Dictionary<string, object> args,
        string? bearer = null,
        Dictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellation = default)
    {
        PrepareClient(bearer, additionalHeaders);

        _ = await httpClient.PostFromJsonAsync<object>(
            url,
            args,
            cancellation);
    }

    /// <summary>
    /// Api Post Json 调用
    /// </summary>
    protected virtual async Task<TOut> CallAsync<TOut>(
        string url,
        Dictionary<string, object> args,
        string? bearer = null,
        bool serialize = false,
        Dictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellation = default)
    {
        PrepareClient(bearer, additionalHeaders);

        return await httpClient.PostFromJsonAsync<TOut>(
            url,
            args,
            cancellation);
    }

    /// <summary>
    /// Api Post Json 调用
    /// </summary>
    protected virtual async Task<TOut> CallAsync<TIn, TOut>(
        string url,
        TIn args,
        string? bearer = null,
        bool serialize = false,
        Dictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellation = default)
    {
        PrepareClient(bearer, additionalHeaders);

        return await httpClient.PostFromJsonAsync<TIn, TOut>(
            url,
            args,
            cancellation);
    }

    /// <summary>
    /// 流式 Api Post Json 调用
    /// </summary>
    protected virtual async IAsyncEnumerable<TOut> CallStreamAsync<TOut>(
        string url,
        Dictionary<string, object> args,
        string? bearer = null,
        Dictionary<string, string>? additionalHeaders = null,
        string streamType = FluentHttpExtensions.EnumerableStream,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        PrepareClient(bearer, additionalHeaders);

        await foreach (var line in httpClient.PostStreamAsync<string>(
            url,
            args,
            streamType,
            cancellation))
        {
            yield return DeserializeStreamLine<TOut>(line);
        }
    }

    /// <summary>
    /// 流式 Api Post Json 调用
    /// </summary>
    protected virtual async IAsyncEnumerable<TOut> CallStreamAsync<TIn, TOut>(
        string url,
        TIn args,
        string? bearer = null,
        Dictionary<string, string>? additionalHeaders = null,
        string streamType = FluentHttpExtensions.EnumerableStream,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        PrepareClient(bearer, additionalHeaders);

        await foreach (var line in httpClient.PostStreamAsync<TIn, string>(
            url,
            args,
            streamType,
            cancellation))
        {
            yield return DeserializeStreamLine<TOut>(line);
        }
    }

    /// <summary>
    /// 流式 Api Post Json 调用
    /// </summary>
    protected virtual async IAsyncEnumerable<TOut> CallStringsAsync<TIn, TOut>(
        string url,
        TIn args,
        string? bearer = null,
        Dictionary<string, string>? additionalHeaders = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        PrepareClient(bearer, additionalHeaders);

        await foreach (var message in httpClient.PostStreamAsync<TIn, TOut>(
            url,
            args,
            FluentHttpExtensions.EnumerableStream,
            cancellation))
        {
            yield return message;
        }
    }
    
    private void PrepareClient(
        string? bearer = null,
        Dictionary<string, string>? additionalHeaders = null)
    {
        if (!string.IsNullOrWhiteSpace(bearer))
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
            httpClient.AddBearerAuthentication(bearer);
        }

        if (additionalHeaders is null || additionalHeaders.Count == 0) return;

        foreach (var (key, value) in additionalHeaders)
        {
            httpClient.DefaultRequestHeaders.Remove(key);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }
    }

    private static TOut DeserializeStreamLine<TOut>(string? line)
    {
        if (typeof(TOut) == typeof(string))
        {
            return (TOut)(object)(line ?? string.Empty);
        }

        var result = JsonSerializer.Deserialize<TOut>(line ?? string.Empty, JsonOptions);

        return result ?? throw new ApplicationException("服务器返回的流式内容无法反序列化。");
    }
}
