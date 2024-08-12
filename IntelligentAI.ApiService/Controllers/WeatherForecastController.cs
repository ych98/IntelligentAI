
using IntelligentAI.ApiService.AspectInjectors;
using IntelligentAI.ApiService.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntelligentAI.ApiService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[ApiExplorerSettings(GroupName = "Standard")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Log(MeasureTime = true)]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet]
    [Log(MeasureTime = true)]
    public IEnumerable<WeatherForecast> GetSleep()
    {
        Thread.Sleep(5000);
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }


    [HttpGet]
    [Log(MeasureTime = true)]
    public async Task<IEnumerable<WeatherForecast>> GetDelayAsync()
    {
        await Task.Delay(5000);
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet]
    [Log]
    public Task GetException()
    {
        Thread.Sleep(5000);
        throw new Exception("test exception.");
    }

    [HttpGet]
    [Log]
    public async Task<IEnumerable<WeatherForecast>> GetApplicationExceptionAsync()
    {
        await Task.Delay(5000);
        throw new ApplicationException("test application exception.");
    }

    [HttpGet]
    public async Task Stream()
    {
        Response.ContentType = "application/octet-stream";

        await foreach (var item in GetData())
        {
            await Response.WriteAsync(item.ToString());
        }

    }

    [HttpGet]
    public async Task StreamText()
    {
        Response.ContentType = "text/event-stream";

        await foreach (var item in GetData())
        {
            await Response.WriteAsync(item.ToString());
        }
        
    }

    [HttpGet]
    public async IAsyncEnumerable<string> StreamEnumerable()
    {
        await foreach (var item in GetData())
        {
            yield return item.ToString();
        }
      
    }

    [HttpGet]
    public async IAsyncEnumerable<int> StreamInt()
    {
        await foreach (var item in GetData())
        {
            yield return item;
        }

    }

    private async IAsyncEnumerable<int> GetData()
    {
        foreach (int item in Enumerable.Range(0, 5))
        {
            await Task.Delay(1000);
            yield return item;
        }
    }
}
