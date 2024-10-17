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

public class AzureAiClient(HttpClient httpClient) : AiClientBase(httpClient)
{
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

        AzureOpenAIClient azureClient = new(
            httpClient.BaseAddress,
            new ApiKeyCredential(ApiKey));

        ChatClient chatClient = azureClient.GetChatClient(ConvertToModelUrl(model.Description));

        ChatCompletion completion = await chatClient.CompleteChatAsync(
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

        AzureOpenAIClient azureClient = new(
            httpClient.BaseAddress,
            new ApiKeyCredential(ApiKey));

        ChatClient chatClient = azureClient.GetChatClient(ConvertToModelUrl(model.Description));

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
        Dictionary<string, object> parameters = new Dictionary<string, object>();

        return parameters;
    }

    private string ConvertToModelUrl(string description)
    {
        return description switch
        {
            ModelEnum.Gpt4oMiniCode => "fanai",
            _ => throw new NotImplementedException($"未实现指定的服务名称模型适配器：{ServiceKey}。")
        };
    }
}