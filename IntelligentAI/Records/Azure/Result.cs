using IntelligentAI.Records.Kimi;
using IntelligentAI.Records.Universal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Records.Azure;

public record AzureResult
(
    [property: System.Text.Json.Serialization.JsonPropertyName("choices")] Choice[] Choices,
    [property: System.Text.Json.Serialization.JsonPropertyName("created")] int Created,
    [property: System.Text.Json.Serialization.JsonPropertyName("id")] string Id,
    [property: System.Text.Json.Serialization.JsonPropertyName("model")] string Model,
    [property: System.Text.Json.Serialization.JsonPropertyName("object")] string Chat,
    [property: System.Text.Json.Serialization.JsonPropertyName("prompt_filter_results")] PromptFilterResults[]? Filters,
    [property: System.Text.Json.Serialization.JsonPropertyName("usage")] Usage? Usage,
    [property: System.Text.Json.Serialization.JsonPropertyName("system_fingerprint")] string? SystemFingerprint
);

public record Usage
(
    [property: System.Text.Json.Serialization.JsonPropertyName("completion_tokens")]int CompletionTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("completion_tokens_details")] CompletionTokensDetails CompletionTokensDetails,
    [property: System.Text.Json.Serialization.JsonPropertyName("prompt_tokens")] int PromptTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("prompt_tokens_details")] PromptTokensDetails PromptTokensDetails,
    [property: System.Text.Json.Serialization.JsonPropertyName("total_tokens")] int TotalTokens
);

public record CompletionTokensDetails
(
    [property: System.Text.Json.Serialization.JsonPropertyName("accepted_prediction_tokens")] int AcceptedPredictionTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("audio_tokens")] int AudioTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("reasoning_tokens")] int ReasoningTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("rejected_prediction_tokens")] int RejectedPredictionTokens
);

public record PromptTokensDetails
(
    [property: System.Text.Json.Serialization.JsonPropertyName("audio_tokens")] int AudioTokens,
    [property: System.Text.Json.Serialization.JsonPropertyName("cached_tokens")] int CachedTokens
);

public record Choice
(
    [property: System.Text.Json.Serialization.JsonPropertyName("content_filter_results")] Dictionary<string, Filter> Filters,
    [property: System.Text.Json.Serialization.JsonPropertyName("finish_reason")] string? FinishReason,
    [property: System.Text.Json.Serialization.JsonPropertyName("index")] int Index, 
    [property: System.Text.Json.Serialization.JsonPropertyName("delta")] Delta? Delta,
    [property: System.Text.Json.Serialization.JsonPropertyName("message")] Message? Message
);

public record Filter
(
    [property: System.Text.Json.Serialization.JsonPropertyName("filtered")] bool Filtered,
    [property: System.Text.Json.Serialization.JsonPropertyName("severity")] string Severity
);

public record PromptFilterResults
(
    [property: System.Text.Json.Serialization.JsonPropertyName("prompt_index")] int PromptIndex,
    [property: System.Text.Json.Serialization.JsonPropertyName("content_filter_results")] Dictionary<string, Filter> Filters
);

