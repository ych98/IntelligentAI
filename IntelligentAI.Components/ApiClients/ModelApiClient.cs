using FluentHttp.Json;
using IntelligentAI.Abstraction;
using IntelligentAI.Enumerations;
using IntelligentAI.Models;
using System.Runtime.CompilerServices;
using System.Threading;

namespace IntelligentAI.Components.ApiClients;

public class ModelApiClient(HttpClient httpClient)
{
    #region Answer
    public async Task<string> AnswerTextAsync(
        AiArguments arguments,
        int modelEnum = 20,
        CancellationToken cancellationToken = default)
    {
        return await httpClient.ReadJsonAsync<string>(
            url: "/Ai/AnswerText".AppendUrl(("modelEnum", modelEnum)),
            method: HttpMethod.Post,
            body: arguments,
            cancellation: cancellationToken);
    }

    public async IAsyncEnumerable<string> AnswerStreamAsync(
        AiArguments arguments,
        int modelEnum = 20,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var message in httpClient.ReadStreamAsync<string>(
            url: "/Ai/AnswerStream".AppendUrl(("modelEnum", modelEnum)),
            method: HttpMethod.Post,
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
        await foreach (var message in httpClient.ReadStreamAsync<string>(
            url: "/Ai/AnswerStrings".AppendUrl(("modelEnum", modelEnum)),
            method: HttpMethod.Post,
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
        await foreach (var message in httpClient.ReadStreamAsync<AiProgressResult>(
            url: "/Ai/AnswerProgress".AppendUrl(("modelEnum", modelEnum)),
            method: HttpMethod.Post,
            requests,
            cancellation: cancellation))
        {
            yield return message;
        }
    }

    #endregion

}
