using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Polly;
using System.Net;
using NLog.Extensions.Logging;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This code is the client equivalent of the ServiceDefaults project. See https://aka.ms/dotnet/aspire/service-defaults
public static class AppDefaultsExtensions
{
    public static MauiAppBuilder AddAppDefaults(this MauiAppBuilder builder)
    {
        builder.ConfigureAppOpenTelemetry();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddResilienceHandler(
                "CustomPipeline",
                static builder =>
                {
                    // See: https://www.pollydocs.org/strategies/retry.html
                    builder.AddRetry(new HttpRetryStrategyOptions
                    {
                        // Customize and configure the retry logic.
                        BackoffType = DelayBackoffType.Exponential,
                        MaxRetryAttempts = 3,
                        UseJitter = true
                    });

                    // See: https://www.pollydocs.org/strategies/circuit-breaker.html
                    builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                    {
                        // Customize and configure the circuit breaker logic.
                        SamplingDuration = TimeSpan.FromSeconds(60 * 10),
                        FailureRatio = 0.2,
                        MinimumThroughput = 3,
                        ShouldHandle = static args =>
                        {
                            return ValueTask.FromResult(args is
                            {
                                Outcome.Result.StatusCode: HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
                            });
                        }
                    });

                    // See: https://www.pollydocs.org/strategies/timeout.html
                    builder.AddTimeout(TimeSpan.FromSeconds(60));
                });

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        builder.AddLog();

        builder.AddAiService();

        return builder;
    }

    public static MauiAppBuilder ConfigureAppOpenTelemetry(this MauiAppBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddRuntimeInstrumentation()
                       .AddAppMeters();
            })
            .WithTracing(tracing =>
            {
                if (builder.Configuration.IsDevelopment())
                {
                    // We want to view all traces in development
                    tracing.SetSampler(new AlwaysOnSampler());
                }

                tracing
                       // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                       //.AddGrpcClientInstrumentation()
                       .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    public static void InitOpenTelemetryServices(this MauiApp mauiApp)
    {
        mauiApp.Services.GetService<MeterProvider>();
        mauiApp.Services.GetService<TracerProvider>();
        // TODO: Uncomment when LoggerProvider is public, with OpenTelemetry.Api version 1.9.0
        //mauiApp.Services.GetService<LoggerProvider>();
    }

    private static MauiAppBuilder AddOpenTelemetryExporters(this MauiAppBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            SetOpenTelemetryEnvironmentVariables();

            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.Exporter package)
        // builder.Services.AddOpenTelemetry()
        //    .UseAzureMonitor();

        return builder;
    }

    

    public static MauiAppBuilder AddLog(this MauiAppBuilder builder)
    {
        builder.Logging.AddSimpleConsole(options => options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ");

        builder.Logging.AddNLog("NLog.config");

        return builder;
    }

    public static MauiAppBuilder AddAiService(this MauiAppBuilder builder)
    {
        SetIntelligentEnvironmentVariables();

        var apiService = builder.Configuration["INTELLIGENTAI_APISERVICE"] ?? "https+http://apiservice";

        var fanewsService = builder.Configuration["INTELLIGENTAI_FANEWSSERVICE"] ?? "https+http://apiservice";

        builder.Services.AddAiService(new AiOptions(apiService));

        builder.Services.AddHttpClient<EventApiClient>(client =>
        {
            client.BaseAddress = new(fanewsService);
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        return builder;
    }

    private static void SetOpenTelemetryEnvironmentVariables()
    {
        foreach (KeyValuePair<string, string> setting in AspireAppSettings.Settings)
        {
            if (setting.Key.StartsWith("OTEL_"))
            {
                Environment.SetEnvironmentVariable(setting.Key, setting.Value);
            }
        }
    }

    private static void SetIntelligentEnvironmentVariables()
    {
        foreach (KeyValuePair<string, string> setting in AppSettings.Settings)
        {
            if (setting.Key.StartsWith("INTELLIGENTAI_"))
            {
                Environment.SetEnvironmentVariable(setting.Key, setting.Value);
            }
        }
    }

    private static MeterProviderBuilder AddAppMeters(this MeterProviderBuilder meterProviderBuilder) =>
        meterProviderBuilder.AddMeter(
            "System.Net.Http");
}
