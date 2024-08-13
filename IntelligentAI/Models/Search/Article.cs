using System.Text.Json.Serialization;

namespace IntelligentAI.Models.Search;

public class Article
{
    //
    // 摘要:
    //     ID, yyyyMMdd四位报纸ID七位当天报纸的流水号
    [JsonPropertyName("articlesequenceid")]
    public string ArticleSequenceId { get; set; }

    //
    // 摘要:
    //     标题相似 @雷哥 这个主要是文章接口里不做特殊处理，因此传过去的和返回的都必须是sameid， 而不是same_id
    public long Sameid { get; set; }

    public string Simhash { get; set; }

    [JsonPropertyName("score")]
    public double Score { get; set; }

    //
    // 摘要:
    //     外部唯一ID
    public string Foreignuniquekey { get; set; }

    //
    // 摘要:
    //     文章ID
    public string Articleid { get; set; }

    //
    // 摘要:
    //     文章类型
    public string Articletype { get; set; }

    //
    // 摘要:
    //     报纸ID
    public int Paperid { get; set; }

    //
    // 摘要:
    //     报纸名称
    public string Papername { get; set; }

    //
    // 摘要:
    //     报纸日期
    public int Paperdate { get; set; }

    //
    // 摘要:
    //     首版 1：代表首版 其他信息：空
    public string Papercount { get; set; }

    //
    // 摘要:
    //     专刊
    public string Specialissue { get; set; }

    //
    // 摘要:
    //     专刊期号
    public string Specialissueno { get; set; }

    //
    // 摘要:
    //     版次
    public string Revision { get; set; }

    //
    // 摘要:
    //     版面
    public string Page { get; set; }

    //
    // 摘要:
    //     责任编辑
    public string Maineditor { get; set; }

    //
    // 摘要:
    //     地区
    public string Region { get; set; }

    //
    // 摘要:
    //     地区编码
    public string Isocode { get; set; }

    //
    // 摘要:
    //     区县ID
    public int Townid { get; set; }

    //
    // 摘要:
    //     分类
    public string Itemtype { get; set; }

    //
    // 摘要:
    //     体裁
    public string Literaturetype { get; set; }

    //
    // 摘要:
    //     人物
    public string Figure { get; set; }

    //
    // 摘要:
    //     来源
    public string Articlesource { get; set; }

    //
    // 摘要:
    //     文章顺序
    public int Articlesort { get; set; }

    //
    // 摘要:
    //     栏目
    public string Typename { get; set; }

    //
    // 摘要:
    //     引题
    public string Parenttitle { get; set; }

    //
    // 摘要:
    //     标题
    public string Title { get; set; }

    //
    // 摘要:
    //     标题字数
    public int Titlewordscount { get; set; }

    //
    // 摘要:
    //     副标题
    public string Subtitle { get; set; }

    //
    // 摘要:
    //     作者
    public string Editor { get; set; }

    //
    // 摘要:
    //     正文内容

    public string Contenttxt { get; set; }

    //
    // 摘要:
    //     正文字数
    public int Contentwordscount { get; set; }

    //
    // 摘要:
    //     文章标记区域
    public string Markinfo { get; set; }

    //
    // 摘要:
    //     全文
    public string Fullcontext { get; set; }

    //
    // 摘要:
    //     文章公开控制
    public int Issue { get; set; }

    //
    // 摘要:
    //     版本说明
    public string Version { get; set; }

    //
    // 摘要:
    //     关键字
    public string Keyword { get; set; }

    //
    // 摘要:
    //     载入URL
    public string Url { get; set; }

    //
    // 摘要:
    //     链接URL
    public string Linkurl { get; set; }

    //
    // 摘要:
    //     内容所在PDF页码
    public string Pdfpageno { get; set; }

    //
    // 摘要:
    //     页面PDF资源文件
    public string Pdfsource { get; set; }

    //
    // 摘要:
    //     页面PDF资源文件字节数
    public long Pdfsize { get; set; }

    //
    // 摘要:
    //     页面JPG资源文件
    public string Layoutsource { get; set; }

    //
    // 摘要:
    //     页面JPG资源文件字节数
    public long Layoutsize { get; set; }

    //
    // 摘要:
    //     版面长宽 格式类型：像素XXXX*XXXX
    public string Layoutwh { get; set; }

    //
    // 摘要:
    //     音频资源文件
    public string Viocesource { get; set; }

    //
    // 摘要:
    //     音频资源文件字节数
    public long Viocesize { get; set; }

    //
    // 摘要:
    //     视频资源文件
    public string Videosource { get; set; }

    //
    // 摘要:
    //     正负面的值 1---10000 ， 1：1%正面 ，10000：100%正面
    public long Videosize { get; set; }

    //
    // 摘要:
    //     格式：资源文件名,字节数; 资源文件名,字节数;
    public string Layoutother { get; set; }

    //
    // 摘要:
    //     版面其它精度资源列表
    public List<LayoutOtherResources> LayoutOtherResources { get; set; }

    //
    // 摘要:
    //     配图文件名文件名 多个配图文件名格式：文件名, 字节数%D%W文件名, 字节数
    public string Imagesource { get; set; }

    //
    // 摘要:
    //     配图文件列表
    public List<ImageSourceInfo> ImageSourceInfo { get; set; }

    //
    // 摘要:
    //     配图文件数
    public int Imagesourcecount { get; set; }

    //
    // 摘要:
    //     配图图片作者 多个配图格式：作者%D%W 作者
    public string Imageeditor { get; set; }

    //
    // 摘要:
    //     配图图片说明 多个插图格式：说明%D%W 说明
    public string Imagecontent { get; set; }

    //
    // 摘要:
    //     配图其它精度资源 多个配图格式：文件名, 字节数; 文件名, 字节数%D%W 文件名, 字节数; 文件名, 字节数
    public string Imageother { get; set; }

    //
    // 摘要:
    //     备用1
    public string Memo1 { get; set; }

    //
    // 摘要:
    //     版面列表URL
    public string Memo2 { get; set; }

    //
    // 摘要:
    //     摘要
    public string Memo3 { get; set; }

    //
    // 摘要:
    //     备用4
    public string Memo4 { get; set; }

    //
    // 摘要:
    //     更新时间
    public DateTime Updatetime { get; set; }

    //
    // 摘要:
    //     创建时间
    public DateTime Createtime { get; set; }

    //
    // 摘要:
    //     创建时间_suoyin
    public DateTime Createindex_time { get; set; }

    //
    // 摘要:
    //     标题相似 @雷哥 相似ID,索引里存储的是 sameid
    public string Same_id { get; set; }

    //
    // 摘要:
    //     内容相似80% @雷哥 媒体热点80% @cfy 相似ID 80% 媒体关注
    public string Sameid1 { get; set; }

    //
    // 摘要:
    //     内容相似85% @雷哥 文章相似度90% @cfy 相似ID 90% 转载分析
    public string Sameid3 { get; set; }

    //
    // 摘要:
    //     分类1
    public int Mediaid { get; set; }

    //
    // 摘要:
    //     分类1
    public string Class1 { get; set; }

    //
    // 摘要:
    //     分类2
    public string Class2 { get; set; }

    //
    // 摘要:
    //     分类3
    public string Class3 { get; set; }

    //
    // 摘要:
    //     采集状态 0：默认 1：删除（就可以再采集做update操作了）
    public int S { get; set; }

    //
    // 摘要:
    //     数据的语言,0：未配置，也代表中文 621 中文 622 英文
    public int Languageid { get; set; }

    //
    // 摘要:
    //     阅读量
    public int Readcount { get; set; }

    //
    // 摘要:
    //     点赞量
    public int Agreecount { get; set; }

    //
    // 摘要:
    //     转发量
    public int Forwardcount { get; set; }

    //
    // 摘要:
    //     评论量
    public int Commentcount { get; set; }

    //
    // 摘要:
    //     收藏量
    public int Collectcount { get; set; }

    //
    // 摘要:
    //     热点文章热度点击量
    public int Clickcount { get; set; }

    //
    // 摘要:
    //     转载数量
    public int Reprintcount { get; set; }

    //
    // 摘要:
    //     转载媒体数量
    public int Reprintmediacount { get; set; }

    public int Remediacount { get; set; }

    public int Rearticlecount { get; set; }

    public int Uvcount { get; set; }

    //
    // 摘要:
    //     来源url 。微信中有一个 外部的原网url。
    public string Sourceurl { get; set; }

    //
    // 摘要:
    //     视屏url地址。
    public string Videourl { get; set; }

    //
    // 摘要:
    //     视屏数量
    public long Vc { get; set; }

    public string Isposts { get; set; }

    public int Istong { get; set; }

    public int Isnfmg { get; set; }

    //
    // 摘要:
    //     媒体号名称
    [JsonPropertyName("mediaaccount")]
    public string MediaAccount { get; set; }

    public double Degree { get; set; }

    [JsonPropertyName("mediarank")]

    public int MediaRank { get; set; }

    //
    // 摘要:
    //     媒体其他属性 类型与es保持一致 tag的值是拼接的 选用long类型
    [JsonPropertyName("tags")]
    public long[] Tags { get; set; }

    [JsonPropertyName("samecount")]
    public string SameCount { get; set; }

    //
    // 摘要:
    //     人名
    [JsonPropertyName("person_nr")]
    public string Person_nr { get; set; }

    //
    // 摘要:
    //     地名
    [JsonPropertyName("place_ns")]
    public string Place_ns { get; set; }

    //
    // 摘要:
    //     组织名
    [JsonPropertyName("org_nt")]
    public string Org_nt { get; set; }

    //
    // 摘要:
    //     领导名
    [JsonPropertyName("person_nrl")]
    public string Person_nrl { get; set; }

    //
    // 摘要:
    //     国家名
    [JsonPropertyName("place_ns1")]
    public string Place_ns1 { get; set; }

    //
    // 摘要:
    //     省份
    [JsonPropertyName("place_ns2")]
    public string Place_ns2 { get; set; }

    //
    // 摘要:
    //     城市
    [JsonPropertyName("place_ns3")]
    public string Place_ns3 { get; set; }

    //
    // 摘要:
    //     观点库-观点
    public string MTopic { get; set; }

    //
    // 摘要:
    //     观点库-来源Id
    public int O_Paperid { get; set; }

    //
    // 摘要:
    //     观点库-来源
    public string O_Papername { get; set; }

    //
    // 摘要:
    //     AI关键词
    public string MKeyword { get; set; }

    //
    // 摘要:
    //     AI 概要
    [JsonPropertyName("aiabs")]
    public string AiAbstraction { get; set; }

    //
    // 摘要:
    //     AI 概要
    [JsonPropertyName("aititle")]
    public string AiTitle { get; set; }

    //
    // 摘要:
    //     AI 事件日期
    [JsonPropertyName("aieventdate")]
    public string AiEventDate { get; set; }
}

public class ImageSourceInfo
{
    //
    // 摘要:
    //     文件名
    public string FileName { get; set; }

    //
    // 摘要:
    //     插图原始图片文件http地址
    public string FileNameHttpOrigin { get; set; }

    //
    // 摘要:
    //     文件字节数
    public long Size { get; set; }

    //
    // 摘要:
    //     配图图片作者 多个配图格式：作者%D%W 作者
    public string ImageEditor { get; set; }

    //
    // 摘要:
    //     配图图片说明 多个插图格式：说明%D%W 说明
    public string ImageContent { get; set; }

    //
    // 摘要:
    //     其它精度图片文件
    public List<FileItemInfo> FileInfo { get; set; }
}

public class FileItemInfo
{
    //
    // 摘要:
    //     文件名
    public string FileName { get; set; }

    //
    // 摘要:
    //     文件字节数
    public long Size { get; set; }
}

//
// 摘要:
//     版面其它精度资源实体。版面JPG与PDF
public class LayoutOtherResources
{
    //
    // 摘要:
    //     页面PDF资源文件
    public string PDFSource { get; set; }

    //
    // 摘要:
    //     页面PDF资源文件字节数
    public long PDFSize { get; set; }

    //
    // 摘要:
    //     页面JPG资源文件
    public string LayoutSource { get; set; }

    //
    // 摘要:
    //     页面JPG资源文件字节数
    public long LayoutSize { get; set; }

    //
    // 摘要:
    //     版面长宽 格式类型：像素XXXX*XXXX
    public string LayoutWH { get; set; }
}

public enum GroupArticleType
{
    //
    // 摘要:
    //     empty
    Empty = 0,
    //
    // 摘要:
    //     报纸
    News = 1,
    //
    // 摘要:
    //     网站
    Website = 2,
    //
    // 摘要:
    //     微信
    Weixin = 3,
    //
    // 摘要:
    //     微博
    Weibo = 4,
    //
    // 摘要:
    //     APP
    Webapp = 5,
    //
    // 摘要:
    //     论坛
    Webbbs = 6,
    //
    // 摘要:
    //     境外媒体
    Weboverseas = 7,
    //
    // 摘要:
    //     独立数据库
    Wemedia = 8,
    //
    // 摘要:
    //     第三方平台入驻媒体号
    Mpaccount = 9,
    //
    // 摘要:
    //     全部
    All = 100
}
