
namespace IntelligentAI.Records.Universal;

public record Message(
    [property: System.Text.Json.Serialization.JsonPropertyName("role")] string Role,
    [property: System.Text.Json.Serialization.JsonPropertyName("content")] string Content);

public record Delta(
    [property: System.Text.Json.Serialization.JsonPropertyName("content")] string? Content);
