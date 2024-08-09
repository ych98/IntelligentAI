
namespace IntelligentAI.Records.Fanews;

public record AiResult(int Status, string Data);

public record AiResult<T>(int Status, T Data);

public record AiImagesResult(int Status, string[] Data);

public record AiVideoResult(AiResultStatus Status, string Result);

public record AiVideoResult<T>(AiResultStatus Status, T Data);

public record AiReplyResult(
    [property: System.Text.Json.Serialization.JsonPropertyName("answer")] string Answer
);

#region 子类

public record ArticlePropertyEmotionKeyValue([property: System.Text.Json.Serialization.JsonPropertyName("first")] string Name, [property: System.Text.Json.Serialization.JsonPropertyName("second")] int Score);

public record ArticlePropertyNerModel(string Name, string Type);

public record ArticlePropertyTripleKeyValue(string Key, string[] Value);

public record AiResultStatus(int Id, [property: System.Text.Json.Serialization.JsonPropertyName("msg")] string Message);

#endregion


