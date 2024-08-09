using IntelligentAI.Enumerations;
using IntelligentAI.Models;
using IntelligentAI.Models.Search;
using IntelligentAI.Records.Fanews;

namespace IntelligentAI.Sdk;

public interface IAiModelService
{

    #region Models

    Task<IEnumerable<ModelEnum>> GetModelsAsync(CancellationToken cancellationToken = default);

    Task<ModelEnum> GetModelByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ModelEnum>> GetModelByNameAsync(string name, CancellationToken cancellationToken = default);

    #endregion

    #region Answer
    Task<string> AnswerTextAsync(
       AiArguments arguments,
       int modelEnum = 12,
       CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> AnswerStreamAsync(
        AiArguments arguments,
        int modelEnum = 12,
        CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> AnswerStringsAsync(
        AiArguments arguments,
        int modelEnum = 12,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<AiProgressResult> AnswerProgressAsync(
        List<AiArguments> requests,
        int modelEnum = 12,
        CancellationToken cancellation = default);

    #endregion

    #region Event
    Task<List<EventVenation>> GetEventsAsync(
        SearchArgs arguments,
        double minScore = 0.53,
        int count = 3,
        string codition = "article",
        CancellationToken cancellationToken = default);

    Task<List<EventVenation>> GetEventsByVectorAsync(
        SearchArgs arguments,
        double minScore = 0.53,
        CancellationToken cancellationToken = default);

    Task<List<EventVenation>> GetEventsByCoreWordsAsync(
        SearchArgs arguments,
        bool analyzeKeyword = true,
        CancellationToken cancellationToken = default);
    #endregion

    #region Prompt
    Task<string> GetPromptAsync(
        string id,
        CancellationToken cancellation = default);

    Task<string> GetPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        CancellationToken cancellation = default);

    Task<string> AnswerTextByPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        int modelEnum = 12,
        CancellationToken cancellation = default);

    IAsyncEnumerable<string> AnswerStringsByPromptAsync(
        string id,
        Dictionary<string, string>? replaces,
        int modelEnum = 12,
        CancellationToken cancellation = default);
    #endregion

    #region Utility
    Task<string[]> DeduplicateAsync(
    string[] strings,
    string methodName = "NormalizedLevenshtein",
    double similarityThreshold = 0.6,
    CancellationToken cancellation = default);

    Task<string[]> GetContentsAsync(
        string html,
        CancellationToken cancellation = default);
    #endregion

    #region Article

    Task<IEnumerable<Article>> GetTopicArticlesAsync(
       SearchArgs arguments,
       CancellationToken cancellationToken = default);

    Task<IEnumerable<Article>> GetArticlesAsync(
        SearchArgs arguments,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Article>> GetVectorArticlesAsync(
        SearchArgs arguments,
        double minScore = 0.53,
        bool sift = false,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Article>> GetArticlesByIdsAsync(
        IEnumerable<long> ids, 
        string? fields = null,
        CancellationToken cancellationToken = default);

    Task<Article> GetArticleByIdAsync(
        long id,
        string? fields = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Business

    #endregion
}
