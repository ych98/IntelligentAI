using IntelligentAI.Aggregates.AiClients;
using IntelligentAI.Aggregates.AiModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentAI.Aggregates;

public class AiClientFactory : IAiClientFactory
{
    public AiClientBase CreateClient(string serviceName, string modelName)
    {
        var serviceKey = $"{serviceName}-{modelName}";

        return AiClientFactoryExtensions.GetModel(serviceKey);
    }

    public AiClientBase CreateClient(int modelId)
    {
        return AiClientFactoryExtensions.GetModel(modelId);
    }

    public IReadOnlyCollection<AIModelSettings> GetClients()
    {
        return AiClientFactoryExtensions.GetModelSettings();
    }
}

public static class AiClientFactoryExtensions
{
    private static readonly Dictionary<int, AiClientBase> ModelFactoriesById = new();
    private static readonly Dictionary<string, AiClientBase> ModelFactoriesByServiceKey = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<int, AIModelSettings> ModelSettingsById = new();

    public static IServiceCollection AddAiClients(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var providers = configuration.GetSection("AI").Get<List<AIProviderSettings>>() ?? new List<AIProviderSettings>();

        RegisterProviders(providers);

        services.AddSingleton<IAiClientFactory, AiClientFactory>();

        return services;
    }

    public static AiClientBase GetModel(int modelId)
    {
        if (ModelFactoriesById.TryGetValue(modelId, out var factory)) return factory;

        throw new InvalidOperationException($"'{modelId}' 不是模型编号的有效值，请确保参数的有效性");
    }

    public static AiClientBase GetModel(string serviceKey)
    {
        if (ModelFactoriesByServiceKey.TryGetValue(serviceKey, out var factory)) return factory;

        throw new InvalidOperationException($"'{serviceKey}' 不是 ServiceKey 的有效值，请确保参数的有效性");
    }

    public static IReadOnlyCollection<AIModelSettings> GetModelSettings()
    {
        return ModelSettingsById.Values
            .OrderBy(model => model.Id)
            .ToArray();
    }

    public static IServiceCollection AddCustomAiClient<TModel>(
        this IServiceCollection services,
        string serviceName,
        AIProviderSettings providerSettings)
        where TModel : AiClientBase
    {
        providerSettings.Service = serviceName;
        RegisterProvider(providerSettings);

        services.AddSingleton<IAiClientFactory, AiClientFactory>();

        return services;
    }

    private static void RegisterProviders(IEnumerable<AIProviderSettings> providers)
    {
        ModelFactoriesById.Clear();
        ModelFactoriesByServiceKey.Clear();
        ModelSettingsById.Clear();

        foreach (var provider in providers)
        {
            RegisterProvider(provider);
        }
    }

    private static void RegisterProvider(AIProviderSettings provider)
    {
        if (provider.Models is null || provider.Models.Count == 0) return;
        if (string.IsNullOrWhiteSpace(provider.Service)) throw new InvalidOperationException("AI 服务配置缺少 Service。");

        var http = CreateHttpClient(provider.Host);

        foreach (var modelSetting in provider.Models)
        {
            modelSetting.Service = provider.Service;

            if (modelSetting.Id <= 0)
            {
                throw new InvalidOperationException($"模型 '{modelSetting.ServiceKey}' 缺少有效 Id。");
            }

            if (ModelFactoriesById.ContainsKey(modelSetting.Id))
            {
                throw new InvalidOperationException($"模型编号 '{modelSetting.Id}' 重复，请确保 AI 配置中的 Id 唯一。");
            }

            var model = CreateClient(provider, modelSetting.Name, http);

            ModelFactoriesById[modelSetting.Id] = model;
            ModelFactoriesByServiceKey[modelSetting.ServiceKey] = model;
            ModelSettingsById[modelSetting.Id] = modelSetting;
        }
    }

    private static AiClientBase CreateClient(
        AIProviderSettings provider,
        string modelName,
        HttpClient http)
    {
        return provider.Service switch
        {
            "Aliyun" => new AliyunAiClient(http, modelName, provider.ApiKey, provider.ChatUrl) { ServiceName = provider.Service },
            "Kimi" => new KimiAiClient(http) { ServiceName = provider.Service, ModelName = modelName, ApiKey = provider.ApiKey, ChatUrl = provider.ChatUrl },
            "Azure" => new AzureAiClient(http, modelName, provider.ApiKey, provider.ChatUrl) { ServiceName = provider.Service },
            "OpenAI" => new OpenAiClient(http, modelName, provider.ApiKey, provider.ChatUrl) { ServiceName = provider.Service },
            "Huoshan" => new HuoshanAiClient(http) { ServiceName = provider.Service, ModelName = modelName, ApiKey = provider.ApiKey, ChatUrl = provider.ChatUrl },
            "Baidu" => new BaiduAiClient(http) { ServiceName = provider.Service, ModelName = modelName, ApiKey = provider.ApiKey, ChatUrl = provider.ChatUrl },
            "Google" => new GoogleAiClient(http) { ServiceName = provider.Service, ModelName = modelName, ApiKey = provider.ApiKey, ChatUrl = provider.ChatUrl },
            _ => throw new NotImplementedException($"未实现指定的服务商模型：{provider.Service}。")
        };
    }

    private static HttpClient CreateHttpClient(string host)
    {
        var handler = new SocketsHttpHandler
        {
            UseProxy = false
        };

        return new HttpClient(handler)
        {
            BaseAddress = new Uri(host)
        };
    }
}
