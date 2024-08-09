namespace IntelligentAI.Enumerations;

public class AnalyzeTypeEnum : Enumeration
{
    public static AnalyzeTypeEnum Emotion = new AnalyzeTypeEnum(1, nameof(Emotion), "情感分析");

    public static AnalyzeTypeEnum CoreWord = new AnalyzeTypeEnum(2, nameof(CoreWord), "关键字");

    public static AnalyzeTypeEnum AbstractContent = new AnalyzeTypeEnum(3, nameof(AbstractContent), "摘要");

    public static AnalyzeTypeEnum Classify = new AnalyzeTypeEnum(4, nameof(Classify), "分类");

    public static AnalyzeTypeEnum Susceptible = new AnalyzeTypeEnum(5, nameof(Susceptible), "敏感");

    public static AnalyzeTypeEnum Circulars = new AnalyzeTypeEnum(6, nameof(Circulars), "通稿");

    public static AnalyzeTypeEnum SensitiveWordJudgment = new AnalyzeTypeEnum(7, nameof(SensitiveWordJudgment), "敏感词判断");

    public static AnalyzeTypeEnum NameExtraction = new AnalyzeTypeEnum(8, nameof(NameExtraction), "名称提取");

    public static AnalyzeTypeEnum SensitiveWordFiltering = new AnalyzeTypeEnum(9, nameof(SensitiveWordFiltering), "敏感词过滤");

    public static AnalyzeTypeEnum SubjectVerbObject = new AnalyzeTypeEnum(10, nameof(SubjectVerbObject), "文章摘要及主谓宾提取");

    public static AnalyzeTypeEnum AbstractContentNew = new AnalyzeTypeEnum(11, nameof(AbstractContentNew), "文章摘要提取");

    public static AnalyzeTypeEnum SpecialCoreWord = new AnalyzeTypeEnum(29, nameof(SpecialCoreWord), "关键词提取，特殊场合用");

    public AnalyzeTypeEnum(int id, string name, string description) : base(id, name, description) { }
    
    public static AnalyzeTypeEnum GetById(int id) => FromId<AnalyzeTypeEnum>(id);

    public static AnalyzeTypeEnum GetByName(string name) => FromName<AnalyzeTypeEnum>(name);

    public static AnalyzeTypeEnum GetByDescription(string description) => FromDescription<AnalyzeTypeEnum>(description);
}
