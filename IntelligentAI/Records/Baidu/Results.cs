
namespace IntelligentAI.Records.Baidu;

public record BaiduResult(
    [property: System.Text.Json.Serialization.JsonPropertyName("id")] string Id,
    [property: System.Text.Json.Serialization.JsonPropertyName("object")] string Object,    
    [property: System.Text.Json.Serialization.JsonPropertyName("created")] int Created,
    [property: System.Text.Json.Serialization.JsonPropertyName("sentence_id")] int? SentenceId,
    [property: System.Text.Json.Serialization.JsonPropertyName("result")] string Result,
    [property: System.Text.Json.Serialization.JsonPropertyName("is_truncated")] bool IsTruncated,
    [property: System.Text.Json.Serialization.JsonPropertyName("is_end")] bool? IsEnd,
    [property: System.Text.Json.Serialization.JsonPropertyName("need_clear_history")] bool ClearHistory,
    [property: System.Text.Json.Serialization.JsonPropertyName("usage")] Usage Usage);

public record Usage(
    [property: System.Text.Json.Serialization.JsonPropertyName("prompt_tokens")] int PromptTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("completion_tokens")] int CompletionTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("total_tokens")] int TotalTokens);