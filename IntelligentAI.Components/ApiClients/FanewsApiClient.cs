using IntelligentAI.Abstraction;
using IntelligentAI.Components.Pages.FanewsGroup;
using IntelligentAI.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IntelligentAI.Components.ApiClients;


public class FanewsApiClient(HttpClient httpClient) : ApiClientBase(httpClient)
{
    public async Task<EventResult[]> GetEventsAsync(Models.Search.SearchArgs arguments, string mode, string including, CancellationToken cancellationToken = default)
    {
        List<EventResult>? events = null;

        string url = $"/FanewsSearch/GetAdditionalEvents?mode={mode}&including={including}&count=3";

        return await CallAsync<Models.Search.SearchArgs, EventResult[]>(
            url,
            arguments,
            cancellation: cancellationToken);
    }
}

public class EventResult
{
    public long Id { get; set; }
    public DateTime Date { get; set; }

    public string Title { get; set; }

    public double Score { get; set; }

    public string Url { get; set; }

    public string Summary { get; set; }

    public string Abstraction { get; set; }

    public string EventDate { get; set; }

    public string Content { get; set; }
}

