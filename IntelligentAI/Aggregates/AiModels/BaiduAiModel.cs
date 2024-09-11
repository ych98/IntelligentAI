using IntelligentAI.Records.Aliyun;
using IntelligentAI.Records.Baidu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntelligentAI.Aggregates.AiModels;

public class BaiduAiModel : AiModelBase
{
    public BaiduAiModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {

    }

    public override async Task<string> AnswerText(
        string question,
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        CancellationToken cancellation = default)
    {

        string promptContent = string.Empty;

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
                        : prompt == PromptEnum.Null
                            ? string.Empty
                            : prompt.Description;
                }
                else
                {
                    promptContent = prompt == PromptEnum.Null
                            ? string.Empty
                            : prompt.Description;
                }
            }
        }

        #endregion

        // 转换为大模型传入参数
        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerText), parameters);

        formatParameters["messages"] = new Records.Universal.Message[]
            {
                new Records.Universal.Message("user", question + "\n" + promptContent)
            };

        var modelUrl = ConvertToModelUrl(model.Description);

        var keys = ApiKey.Split(";");

        var id = keys.First().Substring(keys.First().IndexOf("-") + 1);

        var secret = keys.Last().Substring(keys.Last().IndexOf("-") + 1);

        var token = await GetAsync<BaiduTokenResult>(ApiEnum.BaiduService.Name,
            $"/oauth/2.0/token?grant_type=client_credentials&client_id={id}&client_secret={secret}",
            cancellation: cancellation);

        var aiResult = await CallAsync<Records.Baidu.BaiduResult>(
            ApiEnum.BaiduService.Name,
            $"/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/{modelUrl}?access_token={token.AccessToken}",
            formatParameters,
            cancellation: cancellation);

        return aiResult.Result;
    }

    public override async IAsyncEnumerable<string> AnswerStream(
        string question,
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        string promptContent = string.Empty;

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
                        : prompt == PromptEnum.Null
                            ? string.Empty
                            : prompt.Description;
                }
                else
                {
                    promptContent = prompt == PromptEnum.Null
                            ? string.Empty
                            : prompt.Description;
                }
            }
        }

        #endregion

        // 转换为大模型传入参数
        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerStream), parameters);

        formatParameters["messages"] = new Records.Universal.Message[]
        {
            new Records.Universal.Message("user",question + "\n" + promptContent)
        };

        var modelUrl = ConvertToModelUrl(model.Description);

        var keys = ApiKey.Split(";");

        var id = keys.First().Substring(keys.First().IndexOf("-") + 1);

        var secret = keys.Last().Substring(keys.Last().IndexOf("-") + 1);

        var token = await GetAsync<BaiduTokenResult>(ApiEnum.BaiduService.Name,
            $"/oauth/2.0/token?grant_type=client_credentials&client_id={id}&client_secret={secret}",
            cancellation: cancellation);

        await foreach (var single in CallStreamAsync<string>(
            ApiEnum.BaiduService.Name,
            $"/rpc/2.0/ai_custom/v1/wenxinworkshop/chat/{modelUrl}?access_token={token.AccessToken}",
            formatParameters,
            cancellation: cancellation))
        {
            if (string.IsNullOrWhiteSpace(single)) continue;

            if (!single.StartsWith("data:")) continue;

            string message = single.Substring(single.IndexOf(':') + 1);

            var option = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            string[] result = Array.Empty<string>();

            try
            {
                var reply = System.Text.Json.JsonSerializer.Deserialize<BaiduResult>(message, option);

                if (reply is null || string.IsNullOrWhiteSpace(reply.Result)) continue;

                result = reply.Result.Replace(@"\\", @"\").Split(@"\n");
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
                    // Baidu chat 接口参数
                    {"messages", new List<Records.Universal.Message>()},
                    {"temperature", 0.9},
                    {"penalty_score", 1.0},
                    {"top_p", 0.7}
                },
            nameof(AnswerStream) => new Dictionary<string, object>
                {
                    // Baidu chat 接口参数
                    {"messages", new List<Records.Universal.Message>()},
                    {"temperature", 0.9},
                    {"stream", true},
                    {"penalty_score", 1.0},
                    {"top_p", 0.7}
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
    private Dictionary<string, Dictionary<string, string>> MethodKeyMappings
        => new Dictionary<string, Dictionary<string, string>>
        {
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

    private string ConvertToModelUrl(string description)
    {
        return description switch
        {
            ModelEnum.ErnieSpeedCode => "ernie_speed",
            ModelEnum.ErnieSpeedProCode => "ernie-speed-128k",
            _ => throw new NotImplementedException($"未实现指定的服务名称模型适配器：{ServiceKey}。")
        };
    }
}

public record BaiduTokenResult(
    [property: System.Text.Json.Serialization.JsonPropertyName("refresh_token")] string RefreshToken,
    [property: System.Text.Json.Serialization.JsonPropertyName("expires_in")] int Expires,
    [property: System.Text.Json.Serialization.JsonPropertyName("session_key")] string SessionKey,
    [property: System.Text.Json.Serialization.JsonPropertyName("access_token")] string AccessToken,
    [property: System.Text.Json.Serialization.JsonPropertyName("scope")] string Scope,
    [property: System.Text.Json.Serialization.JsonPropertyName("session_secret")] string SessionSecret);
