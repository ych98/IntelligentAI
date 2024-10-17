using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using IntelligentAI.Components.ApiClients;

namespace IntelligentAI.Client;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder AddCustomServices(this MauiAppBuilder builder)
    {
        builder.AddLog();

        SetIntelligentEnvironmentVariables();

        builder.AddAiService();

        builder.AddFanewsService();

        builder.AddStockService();

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
        var apiService = builder.Configuration["INTELLIGENTAI_APISERVICE"] ?? "https+http://apiservice";

        builder.Services.AddHttpClient<ModelApiClient>(client =>
        {
            client.BaseAddress = new(apiService);
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        return builder;
    }

    public static MauiAppBuilder AddFanewsService(this MauiAppBuilder builder)
    {
        var fanewsService = builder.Configuration["INTELLIGENTAI_FANEWSSERVICE"] ?? "https+http://apiservice";

        builder.Services.AddHttpClient<FanewsApiClient>(client =>
        {
            client.BaseAddress = new(fanewsService);
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        return builder;
    }

    public static MauiAppBuilder AddStockService(this MauiAppBuilder builder)
    {
        var xueqiuService = builder.Configuration["INTELLIGENTAI_XUEQIUSERVICE"] ?? "https+http://apiservice";

        builder.Services.AddHttpClient<XueqiuApiClient>(client =>
        {
            client.BaseAddress = new(xueqiuService);
            client.Timeout = TimeSpan.FromMinutes(1);
        });

        return builder;
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

}
