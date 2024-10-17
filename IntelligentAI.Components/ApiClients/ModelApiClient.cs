using IntelligentAI.Abstraction;
using IntelligentAI.Enumerations;
using IntelligentAI.Models;
using System.Runtime.CompilerServices;

namespace IntelligentAI.Components.ApiClients;

public class ModelApiClient(HttpClient httpClient) : ApiClientBase(httpClient)
{
    #region Answer
    public async Task<string> AnswerTextAsync(
        AiArguments arguments,
        int modelEnum = 12,
        string project = "Default",
        string? projectDescription = null,
        CancellationToken cancellationToken = default)
    {
        string query = ParseQueryString(modelEnum, project, projectDescription);

        string url = $"/Ai/AnswerText?{query}";

        return await CallAsync<AiArguments, string>(           
            url,
            arguments,
            cancellation: cancellationToken);
    }

    public async IAsyncEnumerable<string> AnswerStreamAsync(
        AiArguments arguments,
        int modelEnum = 12,
        string project = "Default",
        string? projectDescription = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string query = ParseQueryString(modelEnum, project, projectDescription);

        // 调用API的URL
        string url = $"/Ai/AnswerStream?{query}";

        await foreach (var message in CallStreamAsync<AiArguments, string>(           
            url,
            arguments,
            cancellation: cancellationToken))
        {
            yield return message;
        }
    }

    public async IAsyncEnumerable<string> AnswerStringsAsync(
        AiArguments arguments,
        int modelEnum = 12,
        string project = "Default",
        string? projectDescription = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string query = ParseQueryString(modelEnum, project, projectDescription);

        string url = $"/Ai/AnswerStrings?{query}";

        await foreach (var message in CallStringsAsync<AiArguments, string>(           
            url,
            arguments,
            cancellation: cancellationToken))
        {
            yield return message;
        }
    }

    public async IAsyncEnumerable<AiProgressResult> AnswerProgressAsync(
        List<AiArguments> requests,
        int modelEnum = 12,
        string project = "Default",
        string? projectDescription = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        string query = ParseQueryString(modelEnum, project, projectDescription);

        string url = $"/Ai/AnswerProgress?{query}";

        await foreach (var message in CallStringsAsync<List<AiArguments>, AiProgressResult>(           
            url,
            requests,
            cancellation: cancellation))
        {
            yield return message;
        }
    }

    #endregion

    private string ParseQueryString(
        int modelEnum = 12,
        string project = "Default",
        string? projectDescription = null)
    {
        string query = string.Empty;

        var urlArguments = System.Web.HttpUtility.ParseQueryString(query);
        urlArguments["modelEnum"] = modelEnum.ToString();
        urlArguments["project"] = project;
        urlArguments["projectDescription"] = projectDescription;
        query = urlArguments.ToString();

        return query;
    }

}
