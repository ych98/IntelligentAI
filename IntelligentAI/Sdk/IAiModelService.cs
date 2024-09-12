using IntelligentAI.Enumerations;
using IntelligentAI.Models;

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
       int modelEnum = 20,
       CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> AnswerStreamAsync(
        AiArguments arguments,
        int modelEnum = 20,
        CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> AnswerStringsAsync(
        AiArguments arguments,
        int modelEnum = 20,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<AiProgressResult> AnswerProgressAsync(
        List<AiArguments> requests,
        int modelEnum = 20,
        CancellationToken cancellation = default);

    #endregion
}
