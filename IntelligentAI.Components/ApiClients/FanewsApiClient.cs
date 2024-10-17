using IntelligentAI.Abstraction;
using System.Runtime.CompilerServices;

namespace IntelligentAI.Components.ApiClients;

public class FanewsApiClient(HttpClient httpClient) : ApiClientBase(httpClient)
{
    public async Task<EventResult[]> GetEventsAsync(
        Models.Search.SearchArgs arguments, 
        int mode = 0,
        int channelId = 0,
        string including = "none",
        string project = "Default",
        CancellationToken cancellationToken = default)
    {
        string query = ParseQueryString(mode, including, channelId, project);

        string url = $"/FanewsSearch/GetAdditionalEvents?{query}";

        return await CallAsync<Models.Search.SearchArgs, EventResult[]>(
            url,
            arguments,
            cancellation: cancellationToken);
    }

    #region Prompt

    public async Task<string> GetPromptAsync(string id, CancellationToken cancellation = default)
    {
        string url = $"/Business/GetPrompt?id={id}";

        return await GetAsync<string>(
            url,
            cancellation: cancellation);
    }


    public async Task<string> GetPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        CancellationToken cancellation = default)
    {
        string url = $"/FanewsBusiness/GetPrompt?id={id}";

        return await CallAsync<Dictionary<string, string>, string>(
            url,
            replaces,
            cancellation: cancellation);
    }

    #endregion

    #region Utility

    public async Task<string[]> DeduplicateAsync(string[] strings, string methodName = "NormalizedLevenshtein", double similarityThreshold = 0.6, CancellationToken cancellation = default)
    {
        string url = $"/Text/Deduplicate?methodName={methodName}&similarityThreshold={similarityThreshold}";

        return await CallAsync<string[], string[]>(
            url,
            strings,
            cancellation: cancellation);
    }

    public async Task<string[]> GetContentsAsync(string html, CancellationToken cancellation = default)
    {
        string url = $"/Html/GetTextArray";

        return await CallAsync<string, string[]>(
            url,
            html,
            cancellation: cancellation);
    }

    public async Task<double[]> GetVectorAsync(string input, CancellationToken cancellation = default)
    {
        string url = $"/Text/GetVector";

        return await CallAsync<string, double[]>(
            url,
            input,
            cancellation: cancellation);
    }


    public async Task<string> GetTranslationAsync(string input, CancellationToken cancellation = default)
    {
        string url = $"/Text/GetTranslation";

        return await CallAsync<string, string>(
            url,
            input,
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
        string url = $"/FanewsBusiness/AnswerTextByPrompt?id={id}&project={project}&modelEnum={modelEnum}";

        var answer = await CallAsync<Dictionary<string, string>, string>(
            url,
            replaces,
            cancellation: cancellation);

        return answer;
    }

    public async IAsyncEnumerable<string> AnswerStringsByPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        int modelEnum = 0,
        string project = "Default",
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        string url = $"/FanewsBusiness/AnswerStringsByPrompt?id={id}&project={project}&modelEnum={modelEnum}";

        await foreach (var message in CallStringsAsync<Dictionary<string, string>, string>(
            url,
            replaces,
            cancellation: cancellation))
        {
            yield return message;
        }
    }

    public async Task<string> GetAiCoreWordsAsync(
       string input,
       CancellationToken cancellationToken = default)
    {
        string url = $"/FanewsAiFunction/GetCoreWords";

        var result = await CallAsync<string, string>(

            url,
            input,
            cancellation: cancellationToken);

        return result;
    }


    #endregion

    #region Analysis

    public async Task<Dictionary<string, HashSet<string>>> GetNameExtractionAsync(
        string content,
        CancellationToken cancellation = default)
    {
        string url = $"/Analyze/GetNameExtraction";

        var answer = await CallAsync<string, Dictionary<string, HashSet<string>>>(

            url,
            content,
            cancellation: cancellation);

        return answer;
    }

    #endregion

    private string ParseQueryString(
    int mode = 4,
    string including = "none",
    int channelId = 0,
    string project = "Default")
    {
        string query = string.Empty;

        var urlArguments = System.Web.HttpUtility.ParseQueryString(query);
        urlArguments["mode"] = mode.ToString();
        urlArguments["including"] = including;
        urlArguments["channelId"] = channelId.ToString();
        urlArguments["project"] = project;
        query = urlArguments.ToString();

        return query;
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

