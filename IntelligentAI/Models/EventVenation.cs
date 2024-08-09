using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Models;

public class EventVenation
{
    public long Id { get; private set; }
    public DateTime Date { get; private set; }
    public string Title { get; private set; }
    public double Score { get; private set; }
    public string? Url { get; set; }

    [System.Text.Json.Serialization.JsonConstructor]
    public EventVenation(
        long id,
        DateTime date,
        string title,
        double score,
        string url = null)
    {
        Id = id;
        Date = date;
        Title = title;
        Score = score;
        Url = url;
    }
}
