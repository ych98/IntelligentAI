using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Abstraction;

public interface IStandardAiFunction
{

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
