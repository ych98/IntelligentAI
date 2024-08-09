using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;

namespace IntelligentAI.Abstraction;

public class ApiBase
{
    protected readonly IHttpClientFactory _httpClientFactory;

    public ApiBase(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public ApiBase()
    {

    }

    /// <summary>
    /// Api Post Form 调用 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="service"></param>
    /// <param name="url"></param>
    /// <param name="args"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    protected virtual async Task<TOut> CallApiAsync<TOut>(string service, string url, Dictionary<string, object> args, CancellationToken cancellation = default)
    {
        // 校验 Api 服务名称
        var api = Enumeration.FromName<FanewsApiEnum>(service);

        using (HttpClient client = _httpClientFactory.CreateClient(api.Name))
        {
            // 将字典转换为键值对集合
            var keyValuePairs = args.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value?.ToString()));

            var content = new FormUrlEncodedContent(keyValuePairs);

            // 发送POST请求
            HttpResponseMessage response = await client.PostAsync(url, content, cancellation);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"服务器返回错误消息: {errorMessage}");
            }

            var result = await response.Content.ReadFromJsonAsync<TOut>();

            return result;
        }

    }

    /// <summary>
    /// 流式 Api Post Form 调用
    /// </summary>
    /// <param name="url"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    protected virtual async IAsyncEnumerable<TOut> CallStreamApiAsync<TOut>(string service, string url, Dictionary<string, object> args, [EnumeratorCancellation] CancellationToken cancellation)
    {
        // 校验 Api 服务名称
        var api = Enumeration.FromName<FanewsApiEnum>(service);

        using (HttpClient client = _httpClientFactory.CreateClient(api.Name))
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // 将字典转换为键值对集合
            var keyValuePairs = args.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value.ToString()));

            request.Content = new FormUrlEncodedContent(keyValuePairs);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellation);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"服务器返回错误消息: {errorMessage}");
            }

            // 打开响应流
            using var stream = await response.Content.ReadAsStreamAsync(cancellation);

            using var reader = new StreamReader(stream);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            // 读取流中的数据
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync(cancellation);

                if (typeof(TOut) == typeof(string))
                {
                    yield return (TOut)(object)line;
                }
                else
                {
                    
                    var result = JsonSerializer.Deserialize<TOut>(line, options);

                    yield return result;
                }


            }
        }

    }


    /// <summary>
    /// Api Get Json 调用 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="service"></param>
    /// <param name="url"></param>
    /// <param name="args"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    protected virtual async Task<TOut> GetAsync<TOut>(
        string service,
        string url,
        string? bearer = null,
        CancellationToken cancellation = default)
    {
        // 校验 Api 服务名称
        var api = Enumeration.FromName<FanewsApiEnum>(service);

        using (HttpClient client = _httpClientFactory.CreateClient(api.Name))
        {
            if (!string.IsNullOrWhiteSpace(bearer))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            }

            // 发送 Get 请求
            HttpResponseMessage response = await client.GetAsync(url, cancellation);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"服务器返回错误消息: {errorMessage}");
            }

            if (typeof(TOut) == typeof(string))
            {
                return (TOut)(object)await response.Content.ReadAsStringAsync(cancellation);
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<TOut>(cancellation);

                return result;
            }

            
        }
    }


    /// <summary>
    /// Api Post Json 调用 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="service"></param>
    /// <param name="url"></param>
    /// <param name="args"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    protected virtual async Task<TOut> CallAsync<TOut>(
        string service,
        string url,
        Dictionary<string, object> args,
        string? bearer = null,
        bool serialize = false,
        Dictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellation = default)
    {
        // 校验 Api 服务名称
        var api = Enumeration.FromName<FanewsApiEnum>(service);

        using (HttpClient client = _httpClientFactory.CreateClient(api.Name))
        {
            if (!string.IsNullOrWhiteSpace(bearer))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            }

            if (additionalHeaders is not null && additionalHeaders.Values.Any())
            {
                foreach (var header in additionalHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            // 发送POST请求
            HttpResponseMessage response;

            if (serialize)
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = System.Text.Json.JsonSerializer.Serialize(args, options);

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                response = await client.PostAsync(url, content, cancellation);
            }
            else
            {                
                response = await client.PostAsJsonAsync(url, args, cancellation);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"服务器返回错误消息: {errorMessage}");
            }

            if (typeof(TOut) == typeof(string))
            {
                return (TOut)(object)await response.Content.ReadAsStringAsync(cancellation);
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<TOut>(cancellation);

                return result;
            }

            
        }
    }

    /// <summary>
    /// Api Post Json 调用 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="service"></param>
    /// <param name="url"></param>
    /// <param name="args"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    protected virtual async Task<TOut> CallAsync<TIn, TOut>(
        string service,
        string url,
        TIn args,
        string? bearer = null,
        bool serialize = false,
        CancellationToken cancellation = default)
    {
        // 校验 Api 服务名称
        var api = Enumeration.FromName<FanewsApiEnum>(service);

        using (HttpClient client = _httpClientFactory.CreateClient(api.Name))
        {
            if (!string.IsNullOrWhiteSpace(bearer))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            }

            // 发送POST请求
            HttpResponseMessage response;

            
            if (serialize)
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = System.Text.Json.JsonSerializer.Serialize(args, options);

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                response = await client.PostAsync(url, content, cancellation);
            }
            else
            {
                response = await client.PostAsJsonAsync(url, args, cancellation);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"服务器返回错误消息: {errorMessage}");
            }

            if (typeof(TOut) == typeof(string))
            {
                return (TOut)(object)await response.Content.ReadAsStringAsync(cancellation);
            }
            else
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadFromJsonAsync<TOut>(options, cancellation);

                return result;
            }

            
        }
    }

    /// <summary>
    /// 流式 Api Post Json 调用
    /// </summary>
    /// <param name="url"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    protected virtual async IAsyncEnumerable<TOut> CallStreamAsync<TOut>(string service, 
        string url, 
        Dictionary<string, object> args, 
        string? bearer = null, 
        Dictionary<string, string>? additionalHeaders = null, 
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        // 校验 Api 服务名称
        var api = Enumeration.FromName<FanewsApiEnum>(service);

        using (HttpClient client = _httpClientFactory.CreateClient(api.Name))
        {
            if (!string.IsNullOrWhiteSpace(bearer))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            }

            if (additionalHeaders is not null && additionalHeaders.Values.Any()) 
            {
                foreach (var header in additionalHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            request.Content = new StringContent(JsonSerializer.Serialize(args, options), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellation);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"服务器返回错误消息: {errorMessage}");
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellation);

            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync(cancellation);

                if (typeof(TOut) == typeof(string))
                {
                    yield return (TOut)(object)line;
                }
                else
                {

                    var result = JsonSerializer.Deserialize<TOut>(line, options);

                    yield return result;
                }
            }
        }

    }

    /// <summary>
    /// 流式 Api Post Json 调用
    /// </summary>
    /// <param name="url"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    protected virtual async IAsyncEnumerable<TOut> CallStreamAsync<TIn,TOut>(string service,
        string url,
        TIn args,
        string? bearer = null,
        Dictionary<string, string>? additionalHeaders = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        // 校验 Api 服务名称
        var api = Enumeration.FromName<FanewsApiEnum>(service);

        using (HttpClient client = _httpClientFactory.CreateClient(api.Name))
        {
            if (!string.IsNullOrWhiteSpace(bearer))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            }

            if (additionalHeaders is not null && additionalHeaders.Values.Any())
            {
                foreach (var header in additionalHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            request.Content = new StringContent(JsonSerializer.Serialize(args, options), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellation);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"服务器返回错误消息: {errorMessage}");
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellation);

            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync(cancellation);
                if (typeof(TOut) == typeof(string))
                {
                    yield return (TOut)(object)line;
                }
                else
                {
                    var result = JsonSerializer.Deserialize<TOut>(line, options);

                    yield return result;
                }                  

            }
        }

    }

    /// <summary>
    /// 流式 Api Post Json 调用
    /// </summary>
    /// <param name="url"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    protected virtual async IAsyncEnumerable<TOut> CallStringsAsync<TIn, TOut>(
        string service,
        string url,
        TIn args,
        string? bearer = null,
        Dictionary<string, string>? additionalHeaders = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        // 校验 Api 服务名称
        var api = Enumeration.FromName<FanewsApiEnum>(service);

        using (HttpClient client = _httpClientFactory.CreateClient(api.Name))
        {
            if (!string.IsNullOrWhiteSpace(bearer))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            }

            if (additionalHeaders is not null && additionalHeaders.Values.Any())
            {
                foreach (var header in additionalHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            request.Content = new StringContent(JsonSerializer.Serialize(args, options), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellation);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"服务器返回错误消息: {errorMessage}");
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellation);

            await foreach (var message in System.Text.Json.JsonSerializer.DeserializeAsyncEnumerable<TOut>(
                stream, 
                options, 
                cancellationToken: cancellation))
            {
                yield return message;
            }
        }

    }
}
