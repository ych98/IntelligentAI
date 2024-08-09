using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IntelligentAI.Models.Search;

public class SearchArgs
{
    //
    // 摘要:
    //     关键词搜索
    public List<Keyword> Keywords { get; set; }

    //
    // 摘要:
    //     小分类数组
    public List<string> Articletypes { get; set; }

    //
    // 摘要:
    //     大分类数组
    public GroupArticleType[] GroupArticletypes { get; set; }

    //
    // 摘要:
    //     城市id列表
    public List<int> Cityids { get; set; }

    //
    // 摘要:
    //     数据源列表
    public List<int> Sourceids { get; set; }

    //
    // 摘要:
    //     媒体id列表
    public List<Media> Mediaids { get; set; }

    //
    // 摘要:
    //     是否返回摘要
    public bool Markinfo { get; set; }

    //
    // 摘要:
    //     文章id列表
    public List<string> Articleids { get; set; }

    //
    // 摘要:
    //     发布日期
    public DateRange Date { get; set; }

    //
    // 摘要:
    //     创建时间
    public DateRange CreateTime { get; set; }

    //
    // 摘要:
    //     发布时间
    public DateRange UpdateTime { get; set; }

    //
    // 摘要:
    //     创建索引时间
    public DateRange CreateIndexTime { get; set; }

    //
    // 摘要:
    //     情感
    public IntRange Emotion { get; set; }

    //
    // 摘要:
    //     原创转载 1-9原创 10-99转载
    public IntRange Original { get; set; }

    //
    // 摘要:
    //     图片数量
    public IntRange ImageCount { get; set; }

    //
    // 摘要:
    //     内容字数
    public IntRange ContentWordsCount { get; set; }

    //
    // 摘要:
    //     返回类似
    public int RetType { get; set; }

    //
    // 摘要:
    //     字段
    public string Fields { get; set; }

    //
    // 摘要:
    //     是否包含视频
    public bool? HasVideo { get; set; }

    //
    // 摘要:
    //     文章分类 社会 教育 时政 经济 军事 科技 体育 艺术 交通 娱乐 汽车 房产 天气 旅游 美食 健康 数码
    public List<string> Classes { get; set; }

    //
    // 摘要:
    //     新情感列表查询 默认或查询 且EmotionAnd=true
    public List<NameRange> EmotionList { get; set; }

    //
    // 摘要:
    //     新情感是否且查询 默认或查询
    public bool EmotionAnd { get; set; }

    //
    // 摘要:
    //     标签搜索
    public List<long> Tags { get; set; }

    //
    // 摘要:
    //     索引前缀
    public string IndexPre { get; set; }

    //
    // 摘要:
    //     其它数据库类型; 如果设置 IndustryTypeId!=None;则：IndexPre="industry_"，因此可以不设置 IndexPre
    public IndustryTypeId IndustryTypeId { get; set; }

    //
    // 摘要:
    //     相似文章 sametype=2标题相似 sametype=3内容相似
    public Same SameTag { get; set; }

    //
    // 摘要:
    //     排序
    public string OrderBy { get; set; } = "updatetime desc";

    public int PageIndex { get; set; } = 1;


    public int PageSize { get; set; } = 20;

}

//
// 摘要:
//     相似度搜索
public class Same
{
    private string _sameid;

    private int _sametype;

    //
    // 摘要:
    //     相似标签 same_id、sameid3、sameid1
    public string Sameid
    {
        get
        {
            return _sameid;
        }
        set
        {
            _sameid = value;
        }
    }

    //
    // 摘要:
    //     sametype=2标题相似 sametype=3内容相似
    public int Sametype
    {
        get
        {
            return _sametype;
        }
        set
        {
            _sametype = value;
        }
    }
}

public enum IndustryTypeId
{
    //
    // 摘要:
    //     通用
    [Description("通用")]
    None = 0,
    //
    // 摘要:
    //     法律库
    [Description("法律库")]
    RegulatoryLibrary = 519,
    //
    // 摘要:
    //     习近平专题
    [Description("习近平专题")]
    XJP = 520,
    //
    // 摘要:
    //     习近平语录
    [Description("习近平语录")]
    XJP_YuLu = 1750,
    //
    // 摘要:
    //     观点库
    [Description("观点库")]
    GDK = 1751
}
public class IntRange
{
    public int From { get; set; }

    public int To { get; set; }

    public IntRange(int from, int to)
    {
        From = from;
        To = to;
    }

    public IntRange()
    {
    }
}

public class NameRange : IntRange
{
    public string Name { get; set; }
}

public class DateRange
{
    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public DateRange(DateTime from, DateTime to)
    {
        From = from;
        To = to;
    }

    public DateRange()
    {
    }
}
public class Keyword
{
    //
    // 摘要:
    //     字段
    [JsonPropertyName("field")]
    public string Field { get; set; }

    //
    // 摘要:
    //     关键词
    [JsonPropertyName("word")]
    public string Word { get; set; }

    //
    // 摘要:
    //     0 and 1 or 2 except
    [JsonPropertyName("andornot")]
    public int AndOrNot { get; set; }
}

public class Media
{
    [JsonPropertyName("mediaid")]
    public int MediaId { get; set; }

    [JsonPropertyName("mediatype")]

    public int MediaType { get; set; }
}