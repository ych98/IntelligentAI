
using IntelligentAI.Records.Universal;

namespace IntelligentAI.Records.Kimi;


public record KimiResult(
    [property: System.Text.Json.Serialization.JsonPropertyName("id")] string Id,
    [property: System.Text.Json.Serialization.JsonPropertyName("object")] string Object,
    [property: System.Text.Json.Serialization.JsonPropertyName("created")] int Created,
    [property: System.Text.Json.Serialization.JsonPropertyName("model")] string Model,
    [property: System.Text.Json.Serialization.JsonPropertyName("choices")] Choice[] Choices,
    [property: System.Text.Json.Serialization.JsonPropertyName("usage")] Usage? Usage);

public record Usage(
    [property: System.Text.Json.Serialization.JsonPropertyName("prompt_tokens")] int PromptTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("completion_tokens")] int CompletionTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("total_tokens")] int TotalTokens);

public record Choice(
    [property: System.Text.Json.Serialization.JsonPropertyName("index")] int Index,
    [property: System.Text.Json.Serialization.JsonPropertyName("message")] Message? Message,
    [property: System.Text.Json.Serialization.JsonPropertyName("delta")] Delta? Delta,
    [property: System.Text.Json.Serialization.JsonPropertyName("finish_reason")] string? FinishReason,
    [property: System.Text.Json.Serialization.JsonPropertyName("usage")] Usage? Usage);

public record Delta(
    [property: System.Text.Json.Serialization.JsonPropertyName("content")] string? Content);