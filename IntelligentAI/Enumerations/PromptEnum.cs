using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Enumerations;

public class PromptEnum : Enumeration
{
    public static PromptEnum Null = new PromptEnum(0, nameof(Null), "无提示词");

    public static PromptEnum Custom = new PromptEnum(1, nameof(Custom), "自定义提示词");

    public static PromptEnum CoreWord = new PromptEnum(2, nameof(CoreWord), "针对以上内容提取不超过10个的关键词,我需要你遵循三个条件：1.不能出现内容中没有的词语。2.提取结果用尖括号包含，用逗号分隔，内容举例：亚运会对杭州经济的影响，结果举例：\"<亚运会>，<杭州>，<经济>\"。3.如果未能提取到关键词，结果不能包含尖括号。");

    public static PromptEnum Title = new PromptEnum(3, nameof(Title), "针对以上内容生成标题，我需要你遵循两个条件：1.生成1到2句话的标题内容，不超过10个字。2.如果第一句话是文章标题，生成的标题和原始标题的相似度不能在6成以上。");

    public static PromptEnum Summary = new PromptEnum(4, nameof(Summary), "你是一个专业的新闻媒体编辑人员，擅长对新闻内容进行精简，你能准确概括发生的核心事件，重点在于发生的事件，而不是发生的时间或产生的数字，精简的具体要求如下： \r\n1、只描述核心事实，不展示新闻具体发生时间或日期，也不展示具体的发生地址；\r\n2、无任何主观评价 ；\r\n3、严禁出现日期、时间、时长、金额等含有数字的信息（除非比赛分数、金融数据、重要政策数据）；\r\n4、对于数据的展现，避免用数字表现，而是笼统中文概括（例如数据占比，百分比，温度等均不能出现具体数字）； \r\n5、不对后续做任何预测 ；\r\n按照上述要求，现在请将下文精简至15字左右，不得超过25字。");

    public static PromptEnum AbstractContent = new PromptEnum(5, nameof(AbstractContent), @"针对以上内容生成概要内容，我需要你遵循两个条件：1.生成3到5句话的概要内容，不超过100字。2.如果第一句话是文章标题，生成的每句概要和标题的相似度不能在6成以上，输出的内容不要包含标题。");

    public static PromptEnum Viewpoint = new PromptEnum(6, nameof(Viewpoint), "你的身份是一名优秀的新闻责编，擅长用简短的语句总结新闻的论点，针对以上内容提取观点，我需要你遵循以下三个条件:\n1.回答的语句严谨且规范；\n2.不能出现国家名称、地区名称和人员姓名；\n3.用几个有意义的词进行核心思想提炼；\n4.用一个有意义的词表示分类。");

    public static PromptEnum FullArticle = new PromptEnum(7, nameof(FullArticle), @"针对以上内容生成一篇完整通顺的文章，我需要你遵循五个条件：1.这些内容是来自不同文章的总结概要，每个句子的后边会有中括号带有的标签表示来自第几篇文章，标签举例：[1]，我需要你在生成文章后保留标签。2.你可以适当的整合内容和扩写句子，但是不要虚构不存在的事件或者内容。3.如果生成文章时你合并了我提供的句子，那么引用时需要保留多个标签，标签举例：[1][3][6]。4.如果你觉得整合后的文章还有可以润色的地方，比如阅读体验感的提高，语句的编排，关键词的改进等，那请给出润色后的文章内容。5.生成的文章字数在500字左右。");

    public static PromptEnum Question = new PromptEnum(8, nameof(Question), "根据内容提出1-3个衍生问题");

    public static PromptEnum Instruction = new PromptEnum(9, nameof(Instruction), "生成不超过5步的操作指南");

    public static PromptEnum Expansion = new PromptEnum(10, nameof(Expansion), "对内容进行扩展，生成不超过100字的解释");

    public static PromptEnum NameExtraction = new PromptEnum(11, nameof(NameExtraction), @"针对以上内容提取人员姓名，我需要你遵循四个条件：1.只提取出现的人员姓名。2.不提取可以概括一类人的名称，比如职务，职业，公民等。3.提取结果用尖括号包含，用逗号分隔，结果举例：<张三>,<李四>,<王五>，未能提取到人员姓名时结果不能包含尖括号。4.不提取组织名称。");
    
    public static PromptEnum Online = new PromptEnum(99, nameof(Online), "凡闻提示词管理库");

    public PromptEnum(int id, string name, string description) : base(id, name, description) { }

    public static PromptEnum GetById(int id) => FromId<PromptEnum>(id);

    public static PromptEnum GetByName(string name) => FromName<PromptEnum>(name);
}
