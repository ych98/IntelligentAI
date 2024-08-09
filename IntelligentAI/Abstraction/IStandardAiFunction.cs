using IntelligentAI.Records.Fanews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Abstraction;

public interface IStandardAiFunction
{
    #region 基础功能
    /// <summary>
    /// 文字生成文字
    /// </summary>
    /// <returns></returns>
    Task<AiResult> AiGetTextFromTextAsync(string content, int count = 1, string type = nameof(PromptEnum.Summary), int modelEnum = 12, CancellationToken cancellation = default);

    /// <summary>
    /// 图片生成文字
    /// </summary>
    /// <returns></returns>
    Task<AiResult> AiGetTextFromImageAsync(string image, int count = 1, string type = nameof(TextFromImageEnum.Content), int modelEnum = 12, CancellationToken cancellation = default);

    /// <summary>
    /// 图片生成图片
    /// </summary>
    /// <returns></returns>
    Task<AiImagesResult> AiGetImageFromImageAsync(string image, string type = nameof(ImageGenerationEnum.New), int modelEnum = 12, CancellationToken cancellation = default);
    
    #endregion

    #region 独立化功能


    /// <summary>
    /// 内容生成标题
    /// </summary>
    /// <param name="content"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<string> AiGenerateTitleAsync(string content, int modelEnum = 12, CancellationToken cancellation = default);

    /// <summary>
    /// 内容生成概要
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    Task<string> AiGenerateAbstractContentAsync(string content, int modelEnum = 12, CancellationToken cancellation = default);

    /// <summary>
    /// 内容生成总结
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    Task<string> AiGenerateSummaryAsync(string content, int modelEnum = 12, CancellationToken cancellation = default);

    /// <summary>
    /// 提问获取关键词
    /// </summary>
    /// <param name="question"></param>
    /// <returns></returns>
    Task<string> AiGetCoreWordsAsync(string question, int modelEnum = 12, CancellationToken cancellation = default);


    #endregion

}
