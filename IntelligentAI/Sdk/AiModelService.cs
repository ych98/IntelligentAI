using IntelligentAI.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

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
            "Intelligence",
            url,
            cancellation: cancellationToken);
    }

    public async Task<ModelEnum> GetModelByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        string url = $"/Ai/AnswerModels";

        var models = await GetAsync<IEnumerable<ModelEnum>>(
            "Intelligence",
            url,
            cancellation: cancellationToken);

        return models.First(m => m.Id == id);
    }

    public async Task<IEnumerable<ModelEnum>> GetModelByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        string url = $"/Ai/AnswerModels";

        var models = await GetAsync<IEnumerable<ModelEnum>>(
            "Intelligence",
            url,
            cancellation: cancellationToken);

        return models.Where(m => m.Description == name);
    }

    #endregion

    #region Answer
    public async Task<string> AnswerTextAsync(
        AiArguments arguments,
        int modelEnum = 20,
        CancellationToken cancellationToken = default)
    {
        string url = $"/Ai/AnswerText?modelEnum={modelEnum}";

        return await CallAsync<AiArguments, string>(
            "Intelligence",
            url,
            arguments,
            cancellation: cancellationToken);
    }

    public async IAsyncEnumerable<string> AnswerStreamAsync(
        AiArguments arguments,
        int modelEnum = 20,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // 调用API的URL
        string url = $"/Ai/AnswerStream?modelEnum={modelEnum}";

        await foreach (var message in CallStreamAsync<AiArguments, string>(
            "Intelligence",
            url,
            arguments,
            cancellation: cancellationToken))
        {
            yield return message;
        }
    }

    public async IAsyncEnumerable<string> AnswerStringsAsync(
        AiArguments arguments,
        int modelEnum = 20,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string url = $"/Ai/AnswerStrings?modelEnum={modelEnum}";

        await foreach (var message in CallStringsAsync<AiArguments, string>(
            "Intelligence",
            url,
            arguments,
            cancellation: cancellationToken))
        {
            yield return message;
        }
    }

    public async IAsyncEnumerable<AiProgressResult> AnswerProgressAsync(
        List<AiArguments> requests,
        int modelEnum = 20,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        string url = $"/AiManager/AnswerProgress?modelEnum={modelEnum}";

        await foreach (var message in CallStringsAsync<List<AiArguments>, AiProgressResult>(
            "Intelligence",
            url,
            requests,
            cancellation: cancellation))
        {
            yield return message;
        }
    }

    #endregion

}

public static class SdkBuilderExtensions
{
    public static IServiceCollection AddAiService(this IServiceCollection services, AiOptions options)
    {
        services.AddHttpClient("Intelligence", (client) =>
        {
            client.BaseAddress = new Uri(options.Address);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton<IAiModelService, AiModelService>();

        return services;
    }
}