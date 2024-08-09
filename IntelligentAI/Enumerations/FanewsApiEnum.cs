using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class FanewsApiEnum : Enumeration
{
    public static FanewsApiEnum FanewsIntelligence = new FanewsApiEnum(0, nameof(FanewsIntelligence), "凡闻智能服务");

    public static FanewsApiEnum AnalyseService = new FanewsApiEnum(1, nameof(AnalyseService), "文章分析服务");

    public static FanewsApiEnum AiAnalyseService = new FanewsApiEnum(2, nameof(AiAnalyseService), "AI分析服务");

    public static FanewsApiEnum AiSearchService = new FanewsApiEnum(3, nameof(AiSearchService), "AI搜索服务");

    public static FanewsApiEnum AiImageService = new FanewsApiEnum(4, nameof(AiImageService), "AI图片服务");

    public static FanewsApiEnum AiVideoService = new FanewsApiEnum(5, nameof(AiVideoService), "AI视频服务");

    public static FanewsApiEnum Wenpluspro = new FanewsApiEnum(6, nameof(Wenpluspro), "智能问答服务");

    public static FanewsApiEnum VectorService = new FanewsApiEnum(7, nameof(VectorService), "向量搜索服务");

    public static FanewsApiEnum EsFullService = new FanewsApiEnum(8, nameof(EsFullService), "关系图谱服务");


    public static FanewsApiEnum KimiService = new FanewsApiEnum(21, nameof(KimiService), "Kimi服务");

    public static FanewsApiEnum AliyunService = new FanewsApiEnum(31, nameof(AliyunService), "Aliyun服务");

    public static FanewsApiEnum HuoshanService = new FanewsApiEnum(41, nameof(HuoshanService), "火山引擎服务");

    public static FanewsApiEnum BaiduService = new FanewsApiEnum(51, nameof(HuoshanService), "百度千帆大模型服务");


    public FanewsApiEnum(int id, string name, string description) : base(id, name, description) { }

    public static FanewsApiEnum GetById(int id) => FromId<FanewsApiEnum>(id);
    
    public static FanewsApiEnum GetByName(string name) => FromName<FanewsApiEnum>(name);
}
