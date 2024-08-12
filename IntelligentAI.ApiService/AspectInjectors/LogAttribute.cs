using AspectInjector.Broker;
using IntelligentAI.ApiService.AspectInjectors.Aspects;
using IntelligentAI.ApiService.AspectInjectors.Attributes;
using IntelligentAI.ApiService.AspectInjectors.Events;
using NLog.Extensions.Logging;
using System.Diagnostics;

namespace IntelligentAI.ApiService.AspectInjectors;

[Injection(typeof(LogAspect))]
public class LogAttribute : MethodAspectAttribute
{
    private readonly ILogger _logger;

    public LogAttribute()
    {
        LoggerFactory = new NLogLoggerFactory();
    }

    public LogAttribute(ILogger logger)
    {
        _logger = logger;
    }

    public static ILoggerFactory LoggerFactory { get; set; }

    // Uses Stopwatch to measure execution time
    public bool MeasureTime { get; set; } = false;

    // Whether to throw exception
    public bool IgnoreException { get; set; } = true;

    public ILogger Logger { get; set; }

    public Stopwatch Timer { get; set; }

    #region Sync

    protected override void OnBefore(AspectEventArgs eventArgs)
    {
        Logger = Logger ?? LoggerFactory.CreateLogger(eventArgs.Name);

        string enteringMessage = eventArgs.Args is null || !eventArgs.Args.Any()
            ? $"Entering method {eventArgs.Name}."
            : $"Entering method {eventArgs.Name} with arguments: {string.Join(", ", eventArgs.Args)}.";

        Logger.LogInformation(enteringMessage);

        if (MeasureTime)
        {
            Timer = Stopwatch.StartNew();
        }

        base.OnBefore(eventArgs);
    }

    protected override void OnAfter(AspectEventArgs eventArgs)
    {
        Logger = Logger ?? LoggerFactory.CreateLogger(eventArgs.Name);

        if (MeasureTime)
        {
            Timer.Stop();

            Logger.LogInformation("Executed method {method} in {time} ms.", eventArgs.Name, Timer.ElapsedMilliseconds);
        }
        else
        {
            Logger.LogInformation("Executed method {method}.", eventArgs.Name);
        }

        base.OnAfter(eventArgs);
    }

    #endregion

    #region Async

    protected override Task OnBeforeAsync(AspectEventArgs eventArgs)
    {
        Logger = Logger ?? LoggerFactory.CreateLogger(eventArgs.Name);

        string enteringMessage = eventArgs.Args is null || !eventArgs.Args.Any()
            ? $"Entering method {eventArgs.Name}."
            : $"Entering method {eventArgs.Name} with arguments: {string.Join(", ", eventArgs.Args)}.";

        Logger.LogInformation(enteringMessage);

        if (MeasureTime)
        {
            Timer = Stopwatch.StartNew();
        }

        return base.OnBeforeAsync(eventArgs);
    }

    protected override Task OnAfterAsync(AspectEventArgs eventArgs)
    {
        Logger = Logger ?? LoggerFactory.CreateLogger(eventArgs.Name);

        if (MeasureTime)
        {
            Timer.Stop();

            Logger.LogInformation("Executed method {method} in {time} ms.", eventArgs.Name, Timer.ElapsedMilliseconds);
        }
        else
        {
            Logger.LogInformation("Executed method {method}.", eventArgs.Name);
        }

        return base.OnAfterAsync(eventArgs);
    }

    #endregion

    #region Exception

    protected override T OnException<T>(AspectEventArgs eventArgs, Exception exception)
    {
        Logger = Logger ?? LoggerFactory.CreateLogger(eventArgs.Name);

        Logger.LogError(exception, $"Error in method {eventArgs.Name}: {exception.Message}");

        if (!IgnoreException) throw exception;

        return default;
    }

    #endregion

}

[Aspect(Scope.PerInstance)]
public class LogAspect : MethodWrapperAspect
{

}