using System.Text.Json.Serialization;

namespace IntelligentAI.Models;

public class AiVideoArguments
{
    public string Question { get; private set; }

    public string Style { get; private set; }

    public int? Width { get; private set; }
    public int? Height { get; private set; }
              
    public int? FramesNumber { get; private set; }
    public int? Fps { get; private set; }
    public int? Step { get; private set; }
    public int? Seed { get; private set; }
    public int? Scale { get; private set; }
    public int? Duration { get; private set; }

    [JsonConstructor]
    public AiVideoArguments(
        string question,
        string style = nameof(VisualizationStyleEnum.Fact),
        int? width = 512,
        int? height = 512,
        int? framesNumber = 24,
        int? fps = 8,
        int? step = 48,
        int? seed = 0,
        int? scale = 2,
        int? duration = 60)
    {
        Question = question;
        Style = style;
        Width = width;
        Height = height;
        FramesNumber = framesNumber;
        Fps = fps;
        Step = step;
        Seed = seed;
        Scale = scale;
        Duration = duration;

    }


    public AiVideoArguments(string question) : this(question, VisualizationStyleEnum.Fact.Name, 512, 512, 24, 8, 48, 0, 2, 60)
    {

    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            {"width", Width},
            {"height", Height},
            {"framesNumber", FramesNumber},
            {"fps", Fps},
            {"step", Step},
            {"seed", Seed},
            {"scale", Scale},
            {"duration", Duration},
            {"style", Style}
        };
    }
}
