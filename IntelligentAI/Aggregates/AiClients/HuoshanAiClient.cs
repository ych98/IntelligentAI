using System.Text.RegularExpressions;

namespace IntelligentAI.Aggregates.AiModels;

public class HuoshanAiClient(HttpClient httpClient) : AiClientBase(httpClient)
{

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
                new Records.Universal.Message("system", "你是一个人工智能助手，擅长中文和英文的对话。你会为用户提供安全，有帮助，准确的回答。"),
                new Records.Universal.Message("user", question + "\n" + promptContent)
            };

        formatParameters["model"] = model.Description;

        var aiResult = await CallAsync<Records.Kimi.KimiResult>("/api/v3/chat/completions", formatParameters, ApiKey,cancellation: cancellation);

        return aiResult.Choices.FirstOrDefault().Message.Content;
    }

    public override async IAsyncEnumerable<string> AnswerStream(string question, Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null, CancellationToken cancellation = default)
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
            new Records.Universal.Message("system", "你是一个人工智能助手，擅长中文和英文的对话。你会为用户提供安全，有帮助，准确的回答。"),
            new Records.Universal.Message("user", question + "\n" + promptContent)
        };

        formatParameters["model"] = ConvertToModelName(model.Description);

        await foreach (var message in CallStreamAsync<string>("/api/v3/chat/completions", formatParameters, ApiKey, cancellation: cancellation))
        {
            // 处理v2版本的接口返回内容
            string pattern = @"""content"":\s*""([^""]*)""";

            List<string> answers = new List<string>();

            foreach (System.Text.RegularExpressions.Match match in Regex.Matches(message, pattern))
            {
                answers.Add(match.Groups[1].Value);
            }

            if (answers.Any())
            {
                string result = string.Join("", answers);

                // 处理接口返回内容中存在的换行符
                var formattedMessages = result.Replace(@"\\", @"\").Split(@"\n");

                foreach (var m in formattedMessages)
                {
                    yield return m;
                }
            }
            else
            {
                yield return "";
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
                    {"model", string.Empty},
                    {"messages", new List<Records.Universal.Message>()},
                    {"temperature", 0.3},
                    {"frequency_penalty", 0},
                    {"top_p", 1.0}
                },
            nameof(AnswerStream) => new Dictionary<string, object>
                {
                    {"model", string.Empty},
                    {"messages", new List<Records.Universal.Message>()},
                    {"temperature", 0.3},
                    {"stream", true},
                    {"frequency_penalty", 0},
                    {"top_p", 1.0}
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

    private string ConvertToModelName(string description) 
    {
        return description switch
        {
            ModelEnum.GLM3Code => "ep-20240619092514-7rrqx",
            ModelEnum.MistralCode => "ep-20240619092424-hnwf9",
            ModelEnum.Llama3Code => "ep-20240619092214-dhjcb",
            _ => throw new NotImplementedException($"未实现指定的服务名称模型适配器：{ServiceKey}。")
        };
    } 
}