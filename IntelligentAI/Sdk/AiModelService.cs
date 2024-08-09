using IntelligentAI.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using IntelligentAI.Records.Fanews;
using IntelligentAI.Models.Search;

namespace IntelligentAI.Sdk;

public class AiModelService : ApiBase, IAiModelService
{
    private readonly IHttpClientFactory _clientFactory;

    public AiModelService(IHttpClientFactory clientFactory) : base(clientFactory)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    }


    #region Models

    public async Task<IEnumerable<ModelEnum>> GetModelsAsync(CancellationToken cancellationToken = default)
    {
        string url = $"/Ai/AnswerModels";

        return await GetAsync<IEnumerable<ModelEnum>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            cancellation: cancellationToken);
    }

    public async Task<ModelEnum> GetModelByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        string url = $"/Ai/AnswerModels";

        var models = await GetAsync<IEnumerable<ModelEnum>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            cancellation: cancellationToken);

        return models.First(m => m.Id == id);
    }

    public async Task<IEnumerable<ModelEnum>> GetModelByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        string url = $"/Ai/AnswerModels";

        var models = await GetAsync<IEnumerable<ModelEnum>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            cancellation: cancellationToken);

        return models.Where(m => m.Description == name);
    }

    #endregion

    #region Answer
    public async Task<string> AnswerTextAsync(
        AiArguments arguments,
        int modelEnum = 12,
        CancellationToken cancellationToken = default)
    {
        string url = $"/Ai/AnswerText?modelEnum={modelEnum}";

        return await CallAsync<AiArguments, string>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellationToken);
    }

    public async IAsyncEnumerable<string> AnswerStreamAsync(
        AiArguments arguments,
        int modelEnum = 12,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // 调用API的URL
        string url = $"/Ai/AnswerStream?modelEnum={modelEnum}";

        await foreach (var message in CallStreamAsync<AiArguments, string>(
            FanewsApiEnum.FanewsIntelligence.Name,
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
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string url = $"/Ai/AnswerStrings?modelEnum={modelEnum}";

        await foreach (var message in CallStringsAsync<AiArguments, string>(
            FanewsApiEnum.FanewsIntelligence.Name,
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
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        string url = $"/AiManager/AnswerProgress?modelEnum={modelEnum}";

        await foreach (var message in CallStringsAsync<List<AiArguments>, AiProgressResult>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            requests,
            cancellation: cancellation))
        {
            yield return message;
        }
    }

    #endregion

    #region Event
    public async Task<List<EventVenation>> GetEventsAsync(
        SearchArgs arguments,
        double minScore = 0.53,
        int count = 3,
        string codition = "article",
        CancellationToken cancellationToken = default)
    {
        string url = $"/FanewsSearch/GetEvents?minScore={minScore}&coreWordsCount={count}&secondaryCondition={codition}";
        return await CallAsync<SearchArgs, List<EventVenation>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellationToken);
    }

    public async Task<List<EventVenation>> GetEventsByCoreWordsAsync(
        SearchArgs arguments,
        bool analyzeKeyword = true,
        CancellationToken cancellationToken = default)
    {
        string url = $"/FanewsSearch/GetEventsByCoreWords?analyzeKeyword={analyzeKeyword}";
        return await CallAsync<SearchArgs, List<EventVenation>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellationToken);
    }

    public async Task<List<EventVenation>> GetEventsByVectorAsync(
        SearchArgs arguments,
        double minScore = 0.53,
        CancellationToken cancellationToken = default)
    {
        string url = $"/FanewsSearch/GetEventsByVector?minScore={minScore}";
        return await CallAsync<SearchArgs, List<EventVenation>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellationToken);
    }

    #endregion

    #region Prompt

    public async Task<string> GetPromptAsync(string id, CancellationToken cancellation = default)
    {
        string url = $"/Business/GetPrompt?id={id}";

        return await GetAsync<string>(
            FanewsApiEnum.FanewsIntelligence.Name,
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
            FanewsApiEnum.FanewsIntelligence.Name,
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
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            strings,
            cancellation: cancellation);
    }

    public async Task<string[]> GetContentsAsync(string html, CancellationToken cancellation = default)
    {
        string url = $"/Html/GetTextArray";

        return await CallAsync<string, string[]>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            html,
            cancellation: cancellation);
    }

    #endregion

    #region Business

    public async Task<string> AnswerTextByPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        int modelEnum = 12,
        CancellationToken cancellation = default)
    {
        string url = $"/FanewsBusiness/GetPrompt?id={id}";

        var question = await CallAsync<Dictionary<string, string>, string>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            replaces,
            cancellation: cancellation);

        url = $"/Ai/AnswerText?modelEnum={modelEnum}";

        AiArguments arguments = new AiArguments(question);

        return await CallAsync<AiArguments, string>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellation);
    }

    public async IAsyncEnumerable<string> AnswerStringsByPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        int modelEnum = 12,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        string url = $"/FanewsBusiness/GetPrompt?id={id}";

        var question = await CallAsync<Dictionary<string, string>, string>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            replaces,
            cancellation: cancellation);

        url = $"/Ai/AnswerStrings?modelEnum={modelEnum}";

        AiArguments arguments = new AiArguments(question);

        await foreach (var message in CallStringsAsync<AiArguments, string>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellation))
        {
            yield return message;
        }

    }

    #endregion


    #region Article

    public async Task<IEnumerable<Article>> GetArticlesAsync(SearchArgs arguments, CancellationToken cancellationToken = default)
    {
        string url = $"/FanewsSearch/GetArticles";

        return await CallAsync<SearchArgs, IEnumerable<Article>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellationToken);
    }

    public async Task<IEnumerable<Article>> GetTopicArticlesAsync(SearchArgs arguments, CancellationToken cancellationToken = default)
    {
        string url = $"/FanewsSearch/GetTopicArticles";
        return await CallAsync<SearchArgs, IEnumerable<Article>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellationToken);
    }

    public async Task<IEnumerable<Article>> GetVectorArticlesAsync(
        SearchArgs arguments,
        double minScore = 0.53,
        bool sift = false,
        CancellationToken cancellationToken = default)
    {
        string url = $"/FanewsSearch/GetVectorArticles?minScore={minScore}&sift={sift}";

        return await CallAsync<SearchArgs, IEnumerable<Article>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            arguments,
            cancellation: cancellationToken);
    }


    public async Task<IEnumerable<Article>> GetArticlesByIdsAsync(IEnumerable<long> ids, string? fields = null, CancellationToken cancellationToken = default)
    {
        string url = $"/Search/GetArticlesByIds?fields={fields}";

        return await CallAsync<IEnumerable<long>, IEnumerable<Article>>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            ids,
            cancellation: cancellationToken);
    }

    public async Task<Article> GetArticleByIdAsync(long id, string? fields = null, CancellationToken cancellationToken = default)
    {
        string url = $"/Search/GetArticle?id={id}&fields={fields}";

        return await GetAsync<Article>(
            FanewsApiEnum.FanewsIntelligence.Name,
            url,
            cancellation: cancellationToken);
    }

    #endregion

}

public static class SdkBuilderExtensions
{
    public static IServiceCollection AddAiService(this IServiceCollection services, AiOptions options)
    {
        services.AddHttpClient(FanewsApiEnum.FanewsIntelligence.Name, (client) =>
        {
            client.BaseAddress = new Uri(options.Address);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton<IAiModelService, AiModelService>();

        return services;
    }
}