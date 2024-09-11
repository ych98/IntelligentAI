using Azure;
using Azure.AI.OpenAI;
using IntelligentAI.Records;
using IntelligentAI.Records.Aliyun;
using IntelligentAI.Records.Universal;
using IntelligentAI.Utilities;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntelligentAI.Aggregates.AiModels;

public class AzureModel : AiModelBase
{
    public AzureModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {

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

        // 校验模型名称
        var model = ModelEnum.GetByDescription(ModelName);

        // 校验传入参数
        if (parameters is not null && parameters.Count > 0)
        {
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

        var messageList = new List<ChatMessage>()
        {
            new SystemChatMessage(
                string.IsNullOrWhiteSpace(systemContent)
                ? "你是一名优秀的人工智能助手，擅长中文和英文的对话。你会为用户提供安全，有帮助，准确的回答。"
                : systemContent)
        };

        if (messages is not null && messages.Any())
        {
            foreach (var m in messages)
            {
                ChatMessage message = m.Role switch
                {
                    "system" => new SystemChatMessage(m.Content),
                    "user" => new UserChatMessage(m.Content),
                    "assistant" => new AssistantChatMessage(m.Content),
                    _ => throw new ArgumentException($"Unknown role: {m.Role}")
                };
                messageList.Add(message);
            }
        }

        var client = _httpClientFactory.CreateClient(FanewsApiEnum.AzureService.Name);

        AzureOpenAIClient azureClient = new(
            client.BaseAddress,
            new AzureKeyCredential(ApiKey));

        ChatClient chatClient = azureClient.GetChatClient(model.Description);

        ChatCompletion completion = chatClient.CompleteChat(
            messageList.Append(new UserChatMessage(HtmlUtilities.GetHtmlValue(TextUtilities.EscapePattern(question)) + "\n\n" + promptContent)));

        return completion.Content.FirstOrDefault()?.Text;
    }

    public override async IAsyncEnumerable<string> AnswerStream(
        string question,
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        string promptContent = string.Empty;

        string systemContent = string.Empty;

        #region 参数校验

        if (string.IsNullOrWhiteSpace(question)) throw new ArgumentNullException($"提问内容不能为空，请确保 question 参数的有效性");

        // 校验模型名称
        var model = ModelEnum.GetByDescription(ModelName);

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
        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerText), parameters);

        var messageList = new List<ChatMessage>()
        {
            new SystemChatMessage(
                string.IsNullOrWhiteSpace(systemContent)
                ? "你是一名优秀的人工智能助手，擅长中文和英文的对话。你会为用户提供安全，有帮助，准确的回答。"
                : systemContent)
        };

        if (messages is not null && messages.Any())
        {
            foreach (var m in messages)
            {
                ChatMessage message = m.Role switch
                {
                    "system" => new SystemChatMessage(m.Content),
                    "user" => new UserChatMessage(m.Content),
                    "assistant" => new AssistantChatMessage(m.Content),
                    _ => throw new ArgumentException($"Unknown role: {m.Role}")
                };
                messageList.Add(message);
            }
        }

        var client = _httpClientFactory.CreateClient(FanewsApiEnum.AzureService.Name);
       
        AzureOpenAIClient azureClient = new(
            client.BaseAddress,
            new AzureKeyCredential(ApiKey));

        ChatClient chatClient = azureClient.GetChatClient(model.Description);

        AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = chatClient.CompleteChatStreamingAsync(
            messageList.Append(
                new UserChatMessage(HtmlUtilities.GetHtmlValue(TextUtilities.EscapePattern(question)) + "\n\n" + promptContent)));

        await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
        {
            foreach (ChatMessageContentPart contentPart in completionUpdate.ContentUpdate)
            {
                yield return contentPart.Text;
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
                    // Aliyun chat 接口参数
                    {"model", "qwen-long"},
                    {"input",new Dictionary<string, object>
                        {
                            {"messages", new List<Records.Universal.Message>()}
                        }},
                    {"parameters", new Dictionary<string, object>
                        {
                            {"result_format", "message"}
                        }}
                },
            nameof(AnswerStream) => new Dictionary<string, object>
                {
                    // Aliyun chat 接口参数
                    {"model", "qwen-long"},
                    {"input", new Dictionary<string, object>
                        {
                            {"messages", new List<Records.Universal.Message>()}
                        }},
                    {"parameters", new Dictionary<string, object>
                        {
                            {"incremental_output", true},
                            {"result_format", "message"}
                        }}
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


    private Dictionary<string, string> AdditionalHeaders(bool stream)
    {
        return ModelName switch
        {
            ModelEnum.AliLongCode => new Dictionary<string, string>()
            {
                ["X-DashScope-SSE"] = stream ? "enable" : "disable",
                ["X-DashScope-DataInspection"] = "{\"input\":\"disable\", \"output\":\"disable\"}"
            },
            ModelEnum.AliPlusCode => new Dictionary<string, string>()
            {
                ["X-DashScope-SSE"] = stream ? "enable" : "disable",
                ["X-DashScope-DataInspection"] = "{\"input\":\"disable\", \"output\":\"disable\"}"
            },
            ModelEnum.AliTurboCode => new Dictionary<string, string>()
            {
                ["X-DashScope-SSE"] = stream ? "enable" : "disable",
                ["X-DashScope-DataInspection"] = "{\"input\":\"disable\", \"output\":\"disable\"}"
            },
            ModelEnum.AliMaxCode => new Dictionary<string, string>()
            {
                ["X-DashScope-SSE"] = stream ? "enable" : "disable",
                ["X-DashScope-DataInspection"] = "{\"input\":\"disable\", \"output\":\"disable\"}"
            },
            _ => new Dictionary<string, string>()
            {
                ["X-DashScope-SSE"] = stream ? "enable" : "disable"
            }
        };

    }
}