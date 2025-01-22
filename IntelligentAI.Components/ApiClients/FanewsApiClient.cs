using FluentHttp.Json;
using IntelligentAI.Abstraction;
using IntelligentAI.Enumerations;
using IntelligentAI.Models;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace IntelligentAI.Components.ApiClients;

public class FanewsApiClient(HttpClient httpClient)
{
    public async Task<EventResult[]> GetEventsAsync(
        Models.Search.SearchArgs arguments, 
        int mode = 0,
        int channelId = 0,
        string including = "none",
        string project = "Default",
        CancellationToken cancellationToken = default)
    {
        return await httpClient.ReadJsonAsync<Models.Search.SearchArgs, EventResult[]>(
            url: "/FanewsSearch/GetAdditionalEvents"
                .AppendUrl(("mode", mode), ("including", including), ("channelId", channelId), ("project", project)),
            method: HttpMethod.Post,
            body: arguments,
            cancellation: cancellationToken);
    }

    #region Prompt

    public async Task<string> GetPromptAsync(string id, CancellationToken cancellation = default)
    {
        return await httpClient.ReadJsonAsync<string>(
            url: "/Business/GetPrompt"
                .AppendUrl(("id", id)),
            method: HttpMethod.Get,
            cancellation: cancellation);
    }


    public async Task<string> GetPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        CancellationToken cancellation = default)
    {
        return await httpClient.ReadJsonAsync< Dictionary<string, string>,string>(
            url: "/FanewsBusiness/GetPrompt"
                .AppendUrl(("id", id)),
            method: HttpMethod.Post,
            body: replaces,
            cancellation: cancellation);
    }

    #endregion

    #region Utility

    public async Task<string[]> DeduplicateAsync(string[] strings, string methodName = "NormalizedLevenshtein", double similarityThreshold = 0.6, CancellationToken cancellation = default)
    {
        return await httpClient.ReadJsonAsync<string[], string[]>(
            url: "/Text/Deduplicate"
                .AppendUrl(("methodName", methodName), ("similarityThreshold", similarityThreshold)),
            method: HttpMethod.Post,
            body: strings,
            cancellation: cancellation);
    }

    public async Task<string[]> GetContentsAsync(string html, CancellationToken cancellation = default)
    {
        return await httpClient.ReadJsonAsync<string, string[]>(
            url: $"/Html/GetTextArray",
            method: HttpMethod.Post,
            body: html,
            cancellation: cancellation);
    }

    public async Task<double[]> GetVectorAsync(string input, CancellationToken cancellation = default)
    {
        return await httpClient.ReadJsonAsync<string, double[]>(
            url: $"/Text/GetVector",
            method: HttpMethod.Post,
            body: input,
            cancellation: cancellation);
    }


    public async Task<string> GetTranslationAsync(string input, CancellationToken cancellation = default)
    {
        return await httpClient.ReadJsonAsync<string, string>(
            url: $"/Text/GetTranslation",
            method: HttpMethod.Post,
            body: input,
            cancellation: cancellation);
    }

    #endregion

    #region Business

    public async Task<string> AnswerTextByPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        int modelEnum = 0,
        string project = "Default",
        CancellationToken cancellation = default)
    {
        return await httpClient.ReadJsonAsync<Dictionary<string, string>, string>(
            url: "/FanewsBusiness/AnswerTextByPrompt"
                .AppendUrl(("id", id), ("project", project), ("modelEnum", modelEnum)),
            method: HttpMethod.Post,
            body: replaces,
            cancellation: cancellation);
    }

    public async IAsyncEnumerable<string> AnswerStringsByPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        int modelEnum = 0,
        string project = "Default",
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        var stream = httpClient.ReadStreamAsync<Dictionary<string, string>, string>(
            url: "/FanewsBusiness/AnswerStringsByPrompt"
                .AppendUrl(("id", id), ("project", project), ("modelEnum", modelEnum)),
            method: HttpMethod.Post,
            body: replaces,
            cancellation: cancellation);

        await foreach (var message in stream)
        {
            yield return message;
        }
    }

    public async Task<string> GetAiCoreWordsAsync(
       string input,
       CancellationToken cancellationToken = default)
    {
        return await httpClient.ReadJsonAsync<string, string>(
            url: "/FanewsAiFunction/GetCoreWords",
            method: HttpMethod.Post,
            body: input,
            cancellation: cancellationToken);

    }


    #endregion

    #region Analysis

    public async Task<Dictionary<string, HashSet<string>>> GetNameExtractionAsync(
        string content,
        CancellationToken cancellation = default)
    {
        return await httpClient.ReadJsonAsync<string, Dictionary<string, HashSet<string>>>(
           url: "/Analyze/GetNameExtraction",
           method: HttpMethod.Post,
           body: content,
           cancellation: cancellation);
    }

    #endregion

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

