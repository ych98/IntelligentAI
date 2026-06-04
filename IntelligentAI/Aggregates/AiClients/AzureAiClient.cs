using FluentHttp.Json;
using IntelligentAI.Records.Azure;
using IntelligentAI.Records.Universal;
using IntelligentAI.Utilities;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace IntelligentAI.Aggregates.AiModels;

public class AzureAiClient(HttpClient httpClient) : AiClientBase(httpClient)
{
    public AzureAiClient(HttpClient httpClient, string modelName, string apiKey, string? chatUrl) : this(httpClient)
    {
        ModelName = modelName;
        ApiKey = apiKey;
        ChatUrl = chatUrl ?? "/openai/deployments/fanai/chat/completions?api-version=2024-08-01-preview";
    }

    public AzureAiClient(HttpClient httpClient, string modelName, string apiKey) : this(httpClient)
    {
        ModelName = modelName;
        ApiKey = apiKey;
    }

    public override async Task<string> AnswerText(
        string question,
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        CancellationToken cancellation = default)
    {

        string promptContent = string.Empty;

        string systemContent = string.Empty;

        #region 参数校验

        if (string.IsNullOrWhiteSpace(question)) throw new ArgumentNullException($"提问内容不能为空，请确保 question 参数的有效性");

        // 校验传入参数
        if (parameters is not null && parameters.Count > 0)
        {
            if (parameters.TryGetValue("topP", out var topP))
            {
                if ((double)topP < 0 || (double)topP > 1) throw new ArgumentOutOfRangeException($"'{(double)topP}' 不是一个有效值，请确保 topP 参数的有效性");
            }

            if (parameters.TryGetValue("temperature", out var temperature))
            {
                if ((double)temperature < 0 || (double)temperature > 1) throw new ArgumentOutOfRangeException($"'{(double)temperature}' 不是一个有效值，请确保 temperature 参数的有效性");
            }

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
                       : string.Empty;
                }
                else
                {
                    promptContent = prompt == PromptEnum.Null || prompt == PromptEnum.System
                            ? string.Empty
                            : prompt.Description;
                }
            }
        }

        #endregion

        // 转换为大模型传入参数
        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerText), parameters);

        var messageList = new List<Message>()
        {
            new Records.Universal.Message("system", string.IsNullOrWhiteSpace(systemContent)
                ? "你是一名优秀的人工智能助手，擅长中文和英文的对话。你会为用户提供安全，有帮助，准确的回答。"
                : systemContent)
        };

        if (messages is not null && messages.Any())
        {
            messageList.AddRange(messages);
        }

        messageList.Add(
                new Message(
                    "user",
                    HtmlUtilities.GetHtmlValue(TextUtilities.EscapePattern(question)) + "\n\n" + promptContent));

        formatParameters["messages"] = messageList;

        var azureResult = await CallAsync<Dictionary<string, object>, AzureResult>(
            url: GetChatUrl(),
            args: formatParameters,
            additionalHeaders: new Dictionary<string, string>
            {
                ["api-key"] = ApiKey
            },
            cancellation: cancellation);

        return azureResult.Choices.FirstOrDefault()?.Message.Content;
    }

    public override async IAsyncEnumerable<string> AnswerStream(string question,
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        string promptContent = string.Empty;
        string systemContent = string.Empty;

        #region 参数校验

        if (string.IsNullOrWhiteSpace(question)) throw new ArgumentNullException($"提问内容不能为空，请确保 question 参数的有效性");

        // 校验传入参数
        if (parameters is not null && parameters.Count > 0)
        {
            if (parameters.TryGetValue("topP", out var topP))
            {
                if ((double)topP < 0 || (double)topP > 1) throw new ArgumentOutOfRangeException($"'{(double)topP}' 不是一个有效值，请确保 topP 参数的有效性");
            }

            if (parameters.TryGetValue("temperature", out var temperature))
            {
                if ((double)temperature <= 0 || (double)temperature >= 1) throw new ArgumentOutOfRangeException($"'{(double)temperature}' 不是一个有效值，请确保 temperature 参数的有效性");
            }

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
                       : string.Empty;

                }
                else
                {
                    promptContent = prompt == PromptEnum.Null || prompt == PromptEnum.System
                            ? string.Empty
                            : prompt.Description;
                }
            }
        }

        #endregion

        // 转换为大模型传入参数
        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerStream), parameters);

        var messageList = new List<Message>()
        {
            new Records.Universal.Message("system", string.IsNullOrWhiteSpace(systemContent)
                ? "你是一名优秀的人工智能助手，擅长中文和英文的对话。你会为用户提供安全，有帮助，准确的回答。"
                : systemContent)
        };

        if (messages is not null && messages.Any())
        {
            messageList.AddRange(messages);
        }

        messageList.Add(
                new Message(
                    "user",
                    HtmlUtilities.GetHtmlValue(TextUtilities.EscapePattern(question)) + "\n\n" + promptContent));

        formatParameters["messages"] = messageList;

        await foreach (var single in CallStreamAsync<Dictionary<string, object>, string>(
            url: GetChatUrl(),
            args: formatParameters,
            additionalHeaders: new Dictionary<string, string>
            {
                ["api-key"] = ApiKey
            },
            streamType: FluentHttpExtensions.EventStream,
            cancellation: cancellation))
        {
            if (string.IsNullOrWhiteSpace(single)) continue;

            if (!single.StartsWith("data:") || single.Trim().EndsWith("[DONE]")) continue;

            string message = single.Substring(single.IndexOf(':') + 1);

            JsonSerializerOptions option = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            string[] result = Array.Empty<string>();

            try
            {
                var reply = System.Text.Json.JsonSerializer.Deserialize<AzureResult>(message, option);

                if (reply is null || reply?.Choices?.FirstOrDefault() is null) continue;

                result = reply?.Choices?.FirstOrDefault()?.Delta?.Content?.Replace(@"\\", @"\").Split(@"\n");
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new ApplicationException($"An error is generated during deserializing process: \n{ex.Message} \n {message}.");
            }

            if (result is null || !result.Any()) continue;

            foreach (var m in result)
            {
                yield return string.IsNullOrEmpty(m) ? "\n" : m;
            }

        }

    }


    public override Task<string[]> AnswerImages(string input, Dictionary<string, object>? parameters = null, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }


    public override Task<string> AnswerVideo(string input, Dictionary<string, object>? parameters = null, CancellationToken cancellation = default)
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
                    {"messages",new List<Records.Universal.Message>()},
                    {"temperature",0.7},
                    {"top_p",0.9},
                    {"frequency_penalty",1},
                    {"presence_penalty",0}
                },
            nameof(AnswerStream) => new Dictionary<string, object>
                {
                    {"messages",new List<Records.Universal.Message>()},
                    {"temperature",0.7},
                    {"top_p",0.9},
                    {"frequency_penalty",1},
                    {"presence_penalty",0},
                    {"stream",true}
                },
            _ => throw new ArgumentException($"Unknown method: {method}")
        };

        if (overrides is null || overrides.Count == 0) return parameters;

        // 获取当前方法的键映射
        var currentMethodKeyMappings = MethodKeyMappings is not null && MethodKeyMappings.ContainsKey(method)
            ? MethodKeyMappings[method]
            : null;

        foreach (var kvp in overrides)
        {
            // 检查是否存在映射关系
            string keyToUse = currentMethodKeyMappings is not null && currentMethodKeyMappings.ContainsKey(kvp.Key)
                ? currentMethodKeyMappings[kvp.Key] :
                kvp.Key;

            // 如果键存在于默认参数中，或者行为是追加，则处理键值对
            if (parameters.ContainsKey(keyToUse) || missingKeyBehavior == MissingKeyBehavior.Append)
            {
                parameters[keyToUse] = kvp.Value;
            }
            else if (missingKeyBehavior == MissingKeyBehavior.Error)
            {
                // 如果键不存在于默认参数中，且行为是报错，则抛出异常
                throw new ArgumentException($"Key not found: {kvp.Key}");
            }
            // 如果行为是忽略，则不做任何操作
        }

        return parameters;
    }

    // 存储每个方法的键映射关系 接口输入参数名称 => 传入大模型参数名称
    private Dictionary<string, Dictionary<string, string>> MethodKeyMappings => new Dictionary<string, Dictionary<string, string>>    {
        {
            nameof(AnswerText), new Dictionary<string, string>
            {
                {"topP", "top_p"}
            }
        },
        {
            nameof(AnswerStream), new Dictionary<string, string>
            {
                {"topP", "top_p"}
            }
        }
    };

    private string GetChatUrl()
    {
        return string.IsNullOrWhiteSpace(ChatUrl)
            ? "/openai/deployments/fanai/chat/completions?api-version=2024-08-01-preview"
            : ChatUrl;
    }
}
