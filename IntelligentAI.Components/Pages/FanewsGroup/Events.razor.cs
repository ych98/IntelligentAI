using IntelligentAI.Enumerations;
using IntelligentAI.SeedWork;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using IntelligentAI.Models;

namespace IntelligentAI.Components.Pages.FanewsGroup;

public class EventResult
{
    public long Id { get; set; }
    public DateTime Date { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    public double Score { get; set; }

    [JsonPropertyName("url")]
    public string SourceUrl { get; set; }

    public string Summary { get; set; }

    [JsonPropertyName("abstraction")]
    public string Abstraction { get; set; }

    public string EventDate { get; set; }

    public string Content { get; set; }
}

public class EventApiClient(HttpClient httpClient)
{
    public async Task<EventResult[]> GetEventsAsync(Models.Search.SearchArgs arguments, string mode, string including, CancellationToken cancellationToken = default)
    {
        List<EventResult>? events = null;

        string url = $"/FanewsSearch/GetAdditionalEvents?mode={mode}&including={including}&count=3";

        return await CallAsync<Models.Search.SearchArgs, EventResult[]>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellationToken);
    }

    private async Task<TOut> CallAsync<TIn, TOut>(
        string service,
        string url,
        TIn args,
        string? bearer = null,
        bool serialize = false,
        CancellationToken cancellation = default)
    {

        // 发送POST请求
        HttpResponseMessage response;

        response = await httpClient.PostAsJsonAsync(url, args, cancellation);


        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();

            throw new ApplicationException($"服务器返回错误消息: {errorMessage}");
        }

        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var result = await response.Content.ReadFromJsonAsync<TOut>(options, cancellation);

        return result;

    }
}