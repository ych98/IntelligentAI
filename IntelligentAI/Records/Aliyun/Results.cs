using IntelligentAI.Records.Universal;

namespace IntelligentAI.Records.Aliyun;

public record AliyunResult(
    [property: System.Text.Json.Serialization.JsonPropertyName("status_code")] int StatusCode,
    [property: System.Text.Json.Serialization.JsonPropertyName("request_id")] string RequestId,
    [property: System.Text.Json.Serialization.JsonPropertyName("code")] string Code,
    [property: System.Text.Json.Serialization.JsonPropertyName("message")] string Message,
    [property: System.Text.Json.Serialization.JsonPropertyName("output")] Output Output,
    [property: System.Text.Json.Serialization.JsonPropertyName("usage")] Usage Usage);

public record Usage(
    [property: System.Text.Json.Serialization.JsonPropertyName("total_tokens")] int TotalTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("output_tokens")] int OutputTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("input_tokens")] int InputTokens);

public record Choice(
    [property: System.Text.Json.Serialization.JsonPropertyName("message")] Message Message,
    [property: System.Text.Json.Serialization.JsonPropertyName("finish_reason")] string FinishReason);

public record Output(
    [property: System.Text.Json.Serialization.JsonPropertyName("text")] string text,
    [property: System.Text.Json.Serialization.JsonPropertyName("finish_reason")] string FinishReason,
    [property: System.Text.Json.Serialization.JsonPropertyName("choices")] Choice[] Choices);
