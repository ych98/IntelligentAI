
using IntelligentAI.Records.Universal;

namespace IntelligentAI.Models;

public class AiArguments
{
    public string Question { get; private set; }

    public string Prompt { get; private set; }
    public string? Template { get; private set; }

    public double? TopP { get; private set; }

    public double? Temperature { get; private set; }

    public int? Paragraphs { get; private set; }

    public Records.Universal.Message[]? Messages { get; private set; }

    [System.Text.Json.Serialization.JsonConstructor]
    public AiArguments(
        string question,
        string prompt = nameof(PromptEnum.Null),
        string? template = null,
        double? topP = 0.8,
        double? temperature = 0.3,
        int? paragraphs = 2,
        Message[]? messages = null)
    {
        Question = question;
        Template = template;
        Prompt = prompt;
        TopP = topP;
        Temperature = temperature;
        Paragraphs = paragraphs;
        Messages = messages;
    }

    // Prompt == PromptEnum.Custom
    public AiArguments(string question, string prompt, string template) : this(question, prompt, template, null, null, null)
    {

    }

    public AiArguments(string question, string prompt, string template, Message[]? messages) : this(question, prompt, template, null, null, null, messages)
    {

    }

    // Prompt != PromptEnum.Null && Prompt != PromptEnum.Custom
    public AiArguments(string question, string prompt) : this(question, prompt, string.Empty)
    {

    }

    public AiArguments(string question, string prompt, Message[]? messages) : this(question, prompt, string.Empty, messages)
    {
        Messages = messages;
    }

    // Prompt == PromptEnum.Null
    public AiArguments(string question) : this(question, PromptEnum.Null.Name)
    {
        if (string.IsNullOrWhiteSpace(question)) throw new ArgumentNullException($"提问内容不能为空，请确保 question 参数的有效性");
    }

    public AiArguments(string question, Message[]? messages) : this(question, PromptEnum.Null.Name, messages)
    {
        if (string.IsNullOrWhiteSpace(question)) throw new ArgumentNullException($"提问内容不能为空，请确保 question 参数的有效性");
    }

    /// <summary>
    /// 将属性转换为字典
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> ToDictionary()
    {
        var parameters = new Dictionary<string, object>()
        {
            { "promptEnum", Prompt }
        };

        if (!string.IsNullOrWhiteSpace(Template))
        {
            parameters.Add("template", Template);
        }

        if (TopP is not null && TopP != 0) 
        {
            parameters.Add("topP", TopP);
        }

        if (Temperature is not null)
        {
            parameters.Add("temperature", Temperature);
        }

        if (Paragraphs is not null)
        {
            parameters.Add("paragraphs", Paragraphs);
        }

        return parameters;

    }

    public override bool Equals(object obj)
    {
        if (obj is AiArguments other)
        {
            // 首先比较 Question
            if (Question != other.Question) return false;

            // 获取当前实例和另一个实例的字典
            var thisDict = ToDictionary();
            var otherDict = other.ToDictionary();

            // 比较字典中的非空键值对
            foreach (var key in thisDict.Keys)
            {
                if (thisDict[key] != null && otherDict.ContainsKey(key))
                {
                    if (!thisDict[key].Equals(otherDict[key])) return false;
                }
            }

            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked // 溢出时不抛出异常
        {
            int hash = 17;
            hash = hash * 23 + Question.GetHashCode();
            var dict = ToDictionary();
            foreach (var key in dict.Keys)
            {
                if (dict[key] != null)
                {
                    hash = hash * 23 + dict[key].GetHashCode();
                }
            }
            return hash;
        }
    }

    public static bool operator ==(AiArguments left, AiArguments right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AiArguments left, AiArguments right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
            WriteIndented = true
        };

        var json = System.Text.Json.JsonSerializer.Serialize(this, options);

        return json;
    }
}