using System.Text.Json.Serialization;

namespace IntelligentAI.Models;

public class AiImagesArguments
{
    public string Question { get; private set; }

    public string Style { get; private set; }

    public int? Width { get; private set; }
    public int? Height { get; private set; }
              
    public int? Quantity { get; private set; }

    [JsonConstructor]
    public AiImagesArguments(
        string question,
        string style = nameof(VisualizationStyleEnum.Fact),
        int? width = 512,
        int? height = 512,
        int? quantity = 1)
    {
        Question = question;
        Style = style;
        Width = width;
        Height = height;
        Quantity = quantity;
    }


    public AiImagesArguments(string question) : this(question, VisualizationStyleEnum.Fact.Name, 512, 512, 2)
    {

    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            {"width", Width},
            {"height", Height},
            {"quantity", Quantity},
            {"style", Style}
        };
    }
}
