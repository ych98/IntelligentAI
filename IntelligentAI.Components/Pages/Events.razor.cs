using System.Text.Json.Serialization;

namespace IntelligentAI.Components.Pages;

public class EventResult
{
    public long Id { get; set; }
    public DateTime Date { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    public double Score { get; set; }

    [JsonPropertyName("url")]
    public string SourceUrl { get; set; }

    public string Summary { get; set; }

    [JsonPropertyName("abstraction")]
    public string Abstraction { get; set; }

    public string EventDate { get; set; }
}
