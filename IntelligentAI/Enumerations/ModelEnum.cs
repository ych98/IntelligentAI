using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class ModelEnum : Enumeration
{
    #region 服务公司名称

    public const string FanewsService = "Fanews";
    public const string OpenAIService = "OpenAI";
    public const string KimiService = "Kimi";
    public const string GoogleService = "Google";
    public const string AliyunService = "Aliyun";
    public const string AzureService = "Azure";

    // 抖音
    public const string HuoshanService = "Huoshan";

    public const string BaiduService = "Baidu";

    #endregion

    #region 模型型号

    #region FanewsService

    // 已弃用

    public const string ShushengModelCode = "internlm2-chat-7b";

    public const string AliMaxModelCode = "qwen:14b";

    public const string AliProModelCode = "qwen:32b";

    // 转发
    public const string AliDefaultModelCode = "Qwen-14B-Chat-Int4";

    // 使用中
    public const string AliModelCode = "qwen2:7b";

    public const string AliProMaxModelCode = "qwen2:72b";

    public const string DeepseekModelCode = "deepseek-v2";

    public const string GoogleModelCode = "gemma2:27b";

    #endregion

    // OpenAIService

    public const string OpenAICode = "gpt-4";

    public const string Gpt4oCode = "gpt-4o";

    // KimiService

    public const string MoonshotCode = "moonshot-v1-8k";

    public const string GoogleCode = "gemini-pro";

    // AliyunService

    public const string AliLongCode = "qwen-long"; 

    public const string AliTurboCode = "qwen-turbo";

    public const string AliPlusCode = "qwen-plus"; 

    public const string AliMaxCode = "qwen-max";

    public const string AliInstructCode = "qwen2-72b-instruct";

    public const string AliBaseInstructCode = "qwen2-7b-instruct";

    // HuoshanService

    public const string GLM3Code = "GLM3";

    public const string MistralCode = "Mistral";

    public const string Llama3Code = "Llama3";

    // BaiduService
    public const string ErnieSpeedCode = "ernie-speed-8k";

    public const string ErnieSpeedProCode = "ernie-speed-128k";

    // AzureService
    public const string Gpt4oMiniCode = "gpt-4o-mini";

    #endregion

    #region 01-10为 Fanews 内部已封装接口

    public static ModelEnum Fanews = new ModelEnum(1, FanewsService, "知识库问答");

    public static ModelEnum GoogleAiModel = new ModelEnum(2, FanewsService, GoogleModelCode);

    public static ModelEnum AliDefaultModel = new ModelEnum(3, FanewsService, AliDefaultModelCode);

    public static ModelEnum DeepseekModel = new ModelEnum(4, FanewsService, DeepseekModelCode);

    public static ModelEnum AliOnlineInstructModel = new ModelEnum(5, FanewsService, AliInstructCode);

    public static ModelEnum AliOnlineLongModel = new ModelEnum(6, FanewsService, AliLongCode);

    public static ModelEnum AliModel = new ModelEnum(7, FanewsService, AliModelCode);

    //public static ModelEnum AliMaxModel = new ModelEnum(8, FanewsService, AliMaxModelCode);

    //public static ModelEnum AliProModel = new ModelEnum(9, FanewsService, AliProModelCode);

    public static ModelEnum AliProMaxModel = new ModelEnum(10, FanewsService, AliProMaxModelCode);

    #endregion

    #region 11-20为国内大模型接口

    public static ModelEnum KimiModel = new ModelEnum(11, KimiService, MoonshotCode);

    public static ModelEnum AliyunModel = new ModelEnum(12, AliyunService, AliLongCode);

    public static ModelEnum AliyunInstructModel = new ModelEnum(13, AliyunService, AliInstructCode);

    public static ModelEnum AliyunMaxModel = new ModelEnum(14, AliyunService, AliMaxCode);

    public static ModelEnum AliyunBaseInstructModel = new ModelEnum(15, AliyunService, AliBaseInstructCode);

    public static ModelEnum AliyunPlusModel = new ModelEnum(16, AliyunService, AliPlusCode);

    public static ModelEnum AliyunTurboModel = new ModelEnum(17, AliyunService, AliTurboCode);

    public static ModelEnum BaiduModel = new ModelEnum(18, BaiduService, ErnieSpeedCode);

    public static ModelEnum BaiduProModel = new ModelEnum(19, BaiduService, ErnieSpeedProCode);

    public static ModelEnum AzureMiniModel = new ModelEnum(20, AzureService, Gpt4oMiniCode);

    #endregion

    #region 21-30为国外大模型接口

    public static ModelEnum OpenAIModel = new ModelEnum(20, OpenAIService, OpenAICode);

    public static ModelEnum Gpt4oModel = new ModelEnum(21, OpenAIService, Gpt4oCode);

    public static ModelEnum GoogleModel = new ModelEnum(25, GoogleService, GoogleCode);

    #endregion

    #region 31-40为火山引擎大模型接口

    public static ModelEnum GLM3Model = new ModelEnum(31, HuoshanService, GLM3Code);

    public static ModelEnum MistralModel = new ModelEnum(32, HuoshanService, MistralCode);

    public static ModelEnum Llama3Model = new ModelEnum(33, HuoshanService, Llama3Code);

    #endregion

    public ModelEnum(int id, string name, string description) : base(id, name, description) { }

    public static ModelEnum GetById(int id) => FromId<ModelEnum>(id);

    // 该枚举类不能通过 Name 获取实例
    // public static ModelEnum GetByName(string name) => FromName<ModelEnum>(name);

    public static ModelEnum GetByDescription(string description) => FromDescription<ModelEnum>(description);
}
