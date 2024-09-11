using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class ModelEnum : Enumeration
{
    #region 服务公司名称

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

    // OpenAIService

    public const string Gpt4Code = "gpt-4";

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

    public static ModelEnum OpenAIModel = new ModelEnum(20, OpenAIService, Gpt4Code);

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
