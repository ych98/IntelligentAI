using System.Runtime.CompilerServices;
using IntelligentAI.Records.Fanews;
using IntelligentAI.Utilities;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace IntelligentAI.Aggregates.AiModels;

public class FanewsAiModel : AiModelBase
{
    public FanewsAiModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {

    }

    /// <summary>
    /// 大模型回答
    /// </summary>
    /// <param name="question"></param>
    /// <param name="template">自定义模板，比 qaType 优先，需要有 $$$question$$$ 。比如：$$$question$$$ \n\n 针对上述语句，生成一篇字数不少于1000字的文章。</param>
    /// <param name="topP">大模型的 topP 参数，越小结果越随机。</param>
    /// <param name="temperature">大模型的 temperature 参数，越大结果越多样。</param>
    /// <param name="score">答案和 content 的相似分数。0：不返回分数，1：返回分数</param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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
                if ((double)topP <= 0 || (double)topP >= 1) throw new ArgumentOutOfRangeException($"'{(double)topP}' 不是一个有效值，请确保 topP 参数的有效性");
            }

            if (parameters.TryGetValue("temperature", out var temperature))
            {
                if ((double)temperature <= 0 || (double)temperature >= 1) throw new ArgumentOutOfRangeException($"'{(double)temperature}' 不是一个有效值，请确保 temperature 参数的有效性");
            }

            if (parameters.TryGetValue("score", out var score))
            {
                if ((int)score < 0 || (int)score > 1) throw new ArgumentOutOfRangeException($"'{(int)score}' 不是一个有效值，请确保 score 参数的有效性");
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

        formatParameters["question"] = TextUtilities.Chunk(HtmlUtilities.GetHtmlValue(TextUtilities.EscapePattern(question)), 1024 * 50) + "\n\n" + promptContent;

        formatParameters["name"] = model.Description;

        var aiResult = await CallApiAsync<AiResult>(FanewsApiEnum.AiSearchService.Name, "/FanAIWeb/qaByChat", formatParameters, cancellation);

        var result = aiResult.Status == 0
            ? aiResult.Data
            : throw new ApplicationException($"An error is generated during the answer generation process:\n {aiResult.Data}.");

        return result;
    }

    /// <summary>
    /// 大模型流式回答
    /// </summary>
    /// <param name="question"></param>
    /// <param name="template">自定义模板，比 qaType 优先，需要有 $$$question$$$ 。比如：$$$question$$$ \n\n 针对上述语句，生成一篇字数不少于1000字的文章。</param>
    /// <param name="topP">大模型的 topP 参数，越小结果越随机。</param>
    /// <param name="temperature">大模型的 temperature 参数，越大结果越多样。</param>
    /// <param name="score">答案和 content 的相似分数。和question匹配的文档相关度阀值，低于这个数的采用。</param>
    /// <param name="paragraphs">段落数 和 question 匹配的文档数。</param>
    /// <param name="apiVersion">流式接口版本，版本为 2 优化了幻觉问题。</param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public override async IAsyncEnumerable<string> AnswerStream(
        string question,
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        string service = string.Empty;

        string url = string.Empty;

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
                if ((double)topP <= 0 || (double)topP >= 1) throw new ArgumentOutOfRangeException($"'{(double)topP}' 不是一个有效值，请确保 topP 参数的有效性");
            }

            if (parameters.TryGetValue("temperature", out var temperature))
            {
                if ((double)temperature <= 0 || (double)temperature >= 1) throw new ArgumentOutOfRangeException($"'{(double)temperature}' 不是一个有效值，请确保 temperature 参数的有效性");
            }

            if (parameters.TryGetValue("score", out var score))
            {
                if ((double)score < 0 || (double)score > 1) throw new ArgumentOutOfRangeException($"'{(double)score}' 不是一个有效值，请确保 score 参数的有效性");
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

            url = (int)parameters["apiVersion"] == 2 ? "/FanAIWeb/qaByStream2" : "/FanAIWeb/qaByStream";
        }

        #endregion

        // 转换为大模型传入参数
        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerStream), parameters);

        formatParameters["pTemplate"] = promptContent;

        var formatQuestion = TextUtilities.Chunk(HtmlUtilities.GetHtmlValue(TextUtilities.EscapePattern(question)), 1024 * 50) + "\n\n" + promptContent;

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        formatParameters["question"] = formatQuestion;

        formatParameters["name"] = model.Description;

        await foreach (var reply in CallStreamApiAsync<AiReplyResult>(
            FanewsApiEnum.AiSearchService.Name, 
            url, 
            formatParameters, 
            cancellation))
        {
            if (reply is null || string.IsNullOrWhiteSpace(reply.Answer)) continue;

            // 处理接口返回内容中存在的换行符
            var formattedMessages = reply.Answer?.Replace(@"\\", @"\").Split(@"\n");

            if (formattedMessages is not null && formattedMessages.Any())
            {
                foreach (var m in formattedMessages)
                {
                    yield return string.IsNullOrWhiteSpace(m) ? "\n" : m;
                }
            }

        }
    }

    public override async Task<string[]> AnswerImages(
        string input,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellation = default)
    {

        #region 参数校验

        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException($"描述内容不能为空，请确保 input 参数的有效性");

        // 校验模型名称
        var model = ModelEnum.GetByDescription(ModelName);

        // 校验传入参数
        if (parameters is not null && parameters.Count > 0)
        {
            if (parameters.TryGetValue("style", out var style))
            {
                // 校验风格
                var imageStyle = VisualizationStyleEnum.GetByDescription((string)style);
            }

            if (parameters.TryGetValue("num", out var num))
            {
                if ((int)num <= 0 || (int)num >= 10) throw new ArgumentOutOfRangeException($"'{(int)num}' 不是一个有效值，请确保 num 参数的有效性");
            }

            if (parameters.TryGetValue("height", out var height))
            {
                if ((int)height <= 0 || (int)height >= 10000) throw new ArgumentOutOfRangeException($"'{(int)height}' 不是一个有效值，请确保 height 参数的有效性");
            }

            if (parameters.TryGetValue("width", out var width))
            {
                if ((int)width <= 0 || (int)width >= 10000) throw new ArgumentOutOfRangeException($"'{(int)width}' 不是一个有效值，请确保 width 参数的有效性");
            }

        }

        #endregion

        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerImages), parameters);

        formatParameters["prompt"] = input; 

        formatParameters["name"] = model.Description;
        var imagesResult =await CallApiAsync<AiImagesResult>(FanewsApiEnum.AiSearchService.Name, "/FanAIWeb/aiGenImage", formatParameters, cancellation);

        var result = imagesResult.Status == 0
            ? imagesResult.Data
            : throw new ApplicationException($"An error is generated during the images generation process:\n {imagesResult.Data}.");

        return result;
    }

    public override async Task<string> AnswerVideo(
        string input,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellation = default)
    {
        #region 参数校验

        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException($"描述内容不能为空，请确保 input 参数的有效性");

        // 校验模型名称
        var model = ModelEnum.GetByDescription(ModelName);

        // 校验传入参数
        if (parameters is not null && parameters.Count > 0)
        {
            if (parameters.TryGetValue("style", out var style))
            {
                // 校验风格
                var imageStyle = VisualizationStyleEnum.GetByDescription((string)style);

                parameters["style"] = imageStyle.Description;
            }

            if (parameters.TryGetValue("height", out var height))
            {
                if ((int)height < 256 || (int)height > 1024) throw new ArgumentOutOfRangeException($"'{(int)height}' 不是一个有效值，请确保 height 参数的有效性");
            }

            if (parameters.TryGetValue("width", out var width))
            {
                if ((int)width < 256 || (int)width > 1024) throw new ArgumentOutOfRangeException($"'{(int)width}' 不是一个有效值，请确保 width 参数的有效性");
            }

            if (parameters.TryGetValue("framesNumber", out var framesNumber))
            {
                if ((int)framesNumber < 1 || (int)framesNumber > 32) throw new ArgumentOutOfRangeException($"'{(int)framesNumber}' 不是一个有效值，请确保 framesNumber 参数的有效性");
            }

            if (parameters.TryGetValue("fps", out var fps))
            {
                if ((int)fps < 1 || (int)fps > 8) throw new ArgumentOutOfRangeException($"'{(int)fps}' 不是一个有效值，请确保 fps 参数的有效性");
            }

            if (parameters.TryGetValue("step", out var step))
            {
                if ((int)step < 5 || (int)step > 50) throw new ArgumentOutOfRangeException($"'{(int)step}' 不是一个有效值，请确保 step 参数的有效性");
            }

            if (parameters.TryGetValue("duration", out var duration))
            {
                if ((int)duration < 0 || (int)duration > 600) throw new ArgumentOutOfRangeException($"'{(int)duration}' 不是一个有效值，请确保 duration 参数的有效性");
            }

            if (parameters.TryGetValue("scale", out var scale))
            {
                if ((int)scale < 0 || (int)scale > 10) throw new ArgumentOutOfRangeException($"'{(int)scale}' 不是一个有效值，请确保 scale 参数的有效性");
            }

        }
        #endregion

        Dictionary<string, object> formatParameters = GetParameters(nameof(AnswerVideo), parameters);

        formatParameters["prompt"] = input;

        var videoResult = await CallAsync<AiVideoResult>(
            FanewsApiEnum.AiVideoService.Name,
            "/mediaservice/txt2video",
            formatParameters,
            string.Empty,
            cancellation: cancellation);

        var result = videoResult.Status.Id == 0
            ? videoResult.Result
            : throw new ApplicationException($"An error is generated during the video generation process:\n Status: {videoResult.Status}, Message: {videoResult.Status.Message}.");

        return result;
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
                    // Fanews qaByChat 接口参数
                    {"question", string.Empty},
                    {"content", string.Empty},
                    {"pattern", "model"},
                    {"pTemplate", string.Empty},
                    {"tp", 0.8},
                    {"tr", 0.3},
                    {"score", 0},
                    {"name", string.Empty}
                },
            nameof(AnswerStream) => new Dictionary<string, object>
                {
                    // Fanews qaByStream 接口参数
                    {"question", string.Empty},
                    {"content", string.Empty},
                    {"pattern", "model"},
                    {"pTemplate", string.Empty},
                    {"tp", 0.8},
                    {"tr", 0.3},
                    {"tk", 2},
                    {"score", 0.6},
                    {"name", string.Empty}
                },
            nameof(AnswerImages) => new Dictionary<string, object>
                {
                    {"prompt", string.Empty},
                    {"num", 1},
                    {"w", 1024},
                    {"h", 1024},
                    {"style", VisualizationStyleEnum.Fact.Description}
                },
            nameof(AnswerVideo) => new Dictionary<string, object>
                {
                    {"prompt", string.Empty},
                    {"width", 512},
                    {"height", 512},
                    {"num_frames", 24},
                    {"fps", 8},
                    {"num_inference_steps", 48},
                    {"seed", 0},
                    {"guidance_scale", 2},
                    {"waittime", 60},
                    {"style", VisualizationStyleEnum.Fact.Description}
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
    private Dictionary<string, Dictionary<string, string>> MethodKeyMappings => new Dictionary<string, Dictionary<string, string>>
    {
        {
            nameof(AnswerText), new Dictionary<string, string>
            {
                // {"template", "pTemplate"},
                {"temperature", "tr"},
                {"topP", "tp"}
            }
        },
        {
            nameof(AnswerStream), new Dictionary<string, string>
            {
                // {"template", "pTemplate"},
                {"temperature", "tr"},
                {"topP", "tp"},
                {"paragraphs", "tk"}
            }
        },
        {
            nameof(AnswerImages), new Dictionary<string, string>
            {
                {"width", "w"},
                {"quantity", "num"},
                {"height", "h"}
            }
        },
        {
            nameof(AnswerVideo), new Dictionary<string, string>
            {
                {"framesNumber", "num_frames"},
                {"step", "num_inference_steps"},
                {"scale", "guidance_scale"},
                {"duration", "waittime"}
            }
        }
    };
}


