using IntelligentAI.Abstraction;
using IntelligentAI.Components.Pages.FanewsGroup;
using IntelligentAI.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntelligentAI.Components.ApiClients;

public class XueqiuApiClient(HttpClient httpClient) : ApiClientBase(httpClient)
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
