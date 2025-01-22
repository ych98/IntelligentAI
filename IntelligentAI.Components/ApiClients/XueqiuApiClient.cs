using FluentHttp.Json;
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

public class XueqiuApiClient(HttpClient httpClient)
{
    public async Task<EventResult[]> GetEventsAsync(Models.Search.SearchArgs arguments, string mode, string including, CancellationToken cancellationToken = default)
    {
        return await httpClient.ReadJsonAsync<Models.Search.SearchArgs, EventResult[]>(
            url: "/FanewsSearch/GetAdditionalEvents".AppendUrl(("mode", mode),("including", including), ("count", 3)),
            method: HttpMethod.Post,
            body: arguments,
            cancellation: cancellationToken);
    }
}
