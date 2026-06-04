using FluentHttp.Json;
using IntelligentAI.Records.Kimi;
using IntelligentAI.Records.Universal;
using IntelligentAI.Utilities;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace IntelligentAI.Aggregates.AiModels;

public class OpenAiClient(HttpClient httpClient) : AiClientBase(httpClient)
{
    private const string DefaultChatUrl = "/v1/chat/completions";
    private const string DefaultSystemPrompt = "你是一个人工智能助手，擅长中文和英文的对话。你会为用户提供安全，有帮助，准确的回答。";

    public OpenAiClient(HttpClient httpClient, string modelName, string apiKey, string? chatUrl = null)
        : this(httpClient)
    {
        ModelName = modelName;
        ApiKey = apiKey;
        ChatUrl = string.IsNullOrWhiteSpace(chatUrl) ? DefaultChatUrl : chatUrl;
    }

    public override async Task<string> AnswerText(
        string question,
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        CancellationToken cancellation = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentNullException(nameof(question), "提问内容不能为空，请确保 question 参数的有效性");
        }

        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerText), parameters);
        formatParameters["model"] = ConvertToModelName(ModelName);
        formatParameters["messages"] = BuildMessages(question, parameters, messages);

        var aiResult = await CallAsync<Dictionary<string, object>, KimiResult>(
            url: GetChatUrl(),
            args: formatParameters,
            bearer: ApiKey,
            cancellation: cancellation);

        return aiResult.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty;
    }

    public override async IAsyncEnumerable<string> AnswerStream(
        string question,
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentNullException(nameof(question), "提问内容不能为空，请确保 question 参数的有效性");
        }

        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerStream), parameters);
        formatParameters["model"] = ConvertToModelName(ModelName);
        formatParameters["messages"] = BuildMessages(question, parameters, messages);

        await foreach (var single in CallStreamAsync<Dictionary<string, object>, string>(
            url: GetChatUrl(),
            args: formatParameters,
            bearer: ApiKey,
            streamType: FluentHttpExtensions.EventStream,
            cancellation: cancellation))
        {
            if (string.IsNullOrWhiteSpace(single)) continue;

            if (!single.StartsWith("data:") || single.Trim().EndsWith("[DONE]")) continue;

            var message = single[(single.IndexOf(':') + 1)..].Trim();

            string[] result;

            try
            {
                var reply = JsonSerializer.Deserialize<KimiResult>(
                    message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var content = reply?.Choices?.FirstOrDefault()?.Delta?.Content;
                if (string.IsNullOrEmpty(content)) continue;

                result = content.Replace(@"\\", @"\").Split(@"\n");
            }
            catch (JsonException ex)
            {
                throw new ApplicationException($"An error is generated during deserializing process: \n{ex.Message} \n {message}.");
            }

            foreach (var item in result)
            {
                yield return string.IsNullOrEmpty(item) ? "\n" : item;
            }
        }
    }

    public override Task<string[]> AnswerImages(
        string input,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task<string> AnswerVideo(
        string input,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    protected override Dictionary<string, object> GetParameters(
        string method,
        Dictionary<string, object>? overrides = null,
        MissingKeyBehavior missingKeyBehavior = MissingKeyBehavior.Ignore)
    {
        Dictionary<string, object> parameters = method switch
        {
            nameof(AnswerText) => new Dictionary<string, object>
            {
                ["model"] = ConvertToModelName(ModelName),
                ["messages"] = new List<Message>(),
                ["temperature"] = 0.3,
                ["presence_penalty"] = 0,
                ["frequency_penalty"] = 0,
                ["top_p"] = 1.0
            },
            nameof(AnswerStream) => new Dictionary<string, object>
            {
                ["model"] = ConvertToModelName(ModelName),
                ["messages"] = new List<Message>(),
                ["temperature"] = 0.3,
                ["stream"] = true,
                ["presence_penalty"] = 0,
                ["frequency_penalty"] = 0,
                ["top_p"] = 1.0
            },
            _ => throw new ArgumentException($"Unknown method: {method}")
        };

        if (overrides is null || overrides.Count == 0) return parameters;

        foreach (var kvp in overrides)
        {
            if (kvp.Key is "promptEnum" or "template") continue;

            var keyToUse = MethodKeyMappings.TryGetValue(method, out var mappings)
                && mappings.TryGetValue(kvp.Key, out var mappedKey)
                    ? mappedKey
                    : kvp.Key;

            if (parameters.ContainsKey(keyToUse) || missingKeyBehavior == MissingKeyBehavior.Append)
            {
                parameters[keyToUse] = kvp.Value;
            }
            else if (missingKeyBehavior == MissingKeyBehavior.Error)
            {
                throw new ArgumentException($"Key not found: {kvp.Key}");
            }
        }

        return parameters;
    }

    protected virtual string ConvertToModelName(string? modelName)
    {
        return modelName switch
        {
            null or "" => throw new InvalidOperationException("ModelName 不能为空。"),
            _ => modelName
        };
    }

    private static readonly Dictionary<string, Dictionary<string, string>> MethodKeyMappings = new()
    {
        [nameof(AnswerText)] = new Dictionary<string, string>
        {
            ["topP"] = "top_p"
        },
        [nameof(AnswerStream)] = new Dictionary<string, string>
        {
            ["topP"] = "top_p"
        }
    };

    private static Message[] BuildMessages(
        string question,
        Dictionary<string, object>? parameters,
        Message[]? messages)
    {
        var promptContent = string.Empty;
        var systemContent = DefaultSystemPrompt;

        if (parameters is not null && parameters.Count > 0)
        {
            ValidateParameters(parameters);

            if (parameters.TryGetValue("promptEnum", out var promptEnum))
            {
                var prompt = Enumeration.FromName<PromptEnum>((string)promptEnum);

                if (parameters.TryGetValue("template", out var template))
                {
                    promptContent = prompt == PromptEnum.Custom
                        ? (string)template
                        : prompt == PromptEnum.Null || prompt == PromptEnum.System
                            ? string.Empty
                            : prompt.Description;

                    systemContent = prompt == PromptEnum.System
                        ? (string)template
                        : DefaultSystemPrompt;
                }
                else
                {
                    promptContent = prompt == PromptEnum.Null || prompt == PromptEnum.System
                        ? string.Empty
                        : prompt.Description;
                }
            }
        }

        var messageList = new List<Message>
        {
            new("system", systemContent)
        };

        if (messages is not null && messages.Any())
        {
            messageList.AddRange(messages);
        }

        messageList.Add(new Message(
            "user",
            HtmlUtilities.GetHtmlValue(TextUtilities.EscapePattern(question)) + "\n\n" + promptContent));

        return messageList.ToArray();
    }

    private static void ValidateParameters(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("topP", out var topP))
        {
            if (Convert.ToDouble(topP) < 0 || Convert.ToDouble(topP) > 1)
            {
                throw new ArgumentOutOfRangeException($"'{Convert.ToDouble(topP)}' 不是一个有效值，请确保 topP 参数的有效性");
            }
        }

        if (parameters.TryGetValue("temperature", out var temperature))
        {
            if (Convert.ToDouble(temperature) < 0 || Convert.ToDouble(temperature) > 1)
            {
                throw new ArgumentOutOfRangeException($"'{Convert.ToDouble(temperature)}' 不是一个有效值，请确保 temperature 参数的有效性");
            }
        }
    }

    private string GetChatUrl()
    {
        return string.IsNullOrWhiteSpace(ChatUrl) ? DefaultChatUrl : ChatUrl;
    }
}
