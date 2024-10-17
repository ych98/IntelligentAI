using IntelligentAI.Aggregates.AiClients;
using IntelligentAI.Aggregates.AiModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentAI.Aggregates;

public class AiClientFactory : IAiClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    public AiClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AiClientBase CreateClient(string serviceName, string modelName)
    {
        var serviceKey = $"{serviceName}-{modelName}";

        return AiClientFactoryExtensions.GetModel(_serviceProvider, serviceKey);
    }

    public AiClientBase CreateClient(int modelEnum)
    {
        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var serviceKey = $"{modelInformation.Name}-{modelInformation.Description}";

        return AiClientFactoryExtensions.GetModel(_serviceProvider, serviceKey);
    }
}

public static class AiClientFactoryExtensions
{
    private static readonly Dictionary<string, AiClientBase> _modelFactories = new Dictionary<string, AiClientBase>();

    public static IServiceCollection AddAiClients(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var modelServices = ServiceEnum.GetAll<ServiceEnum>();

        foreach (var service in modelServices)
        {
            var settings = configuration.GetSection($"AI:{service.Name}").Get<AIProviderSettings>();

            if (settings is null || settings.Models is null || settings.Models.Count == 0) continue;

            var http = new HttpClient() { BaseAddress = new Uri(settings.Host) };

            foreach (var modelName in settings.Models)
            {
                AiClientBase model = service switch
                {
                    var s when s == ServiceEnum.Aliyun => new AliyunAiClient(http) { ServiceName = service.Name, ModelName = modelName, ApiKey = settings.ApiKey },
                    var s when s == ServiceEnum.Kimi => new KimiAiClient(http) { ServiceName = service.Name, ModelName = modelName, ApiKey = settings.ApiKey },
                    var s when s == ServiceEnum.Azure => new AzureAiClient(http) { ServiceName = service.Name, ModelName = modelName, ApiKey = settings.ApiKey },
                    var s when s == ServiceEnum.OpenAI => new OpenAiClient(http) { ServiceName = service.Name, ModelName = modelName, ApiKey = settings.ApiKey },
                    var s when s == ServiceEnum.Huoshan => new HuoshanAiClient(http) { ServiceName = service.Name, ModelName = modelName, ApiKey = settings.ApiKey },
                    var s when s == ServiceEnum.Baidu => new BaiduAiClient(http) { ServiceName = service.Name, ModelName = modelName, ApiKey = settings.ApiKey },
                    var s when s == ServiceEnum.Google => new GoogleAiClient(http) { ServiceName = service.Name, ModelName = modelName, ApiKey = settings.ApiKey },
                    _ => throw new NotImplementedException($"未实现指定的服务商模型：{service.Name}。")
                };

                _modelFactories[model.ServiceKey] = model;
            }
        }

        // 注册IAiClientFactory
        services.AddSingleton<IAiClientFactory>(serviceProvider => new AiClientFactory(serviceProvider));

        return services;
    }

    public static AiClientBase GetModel(IServiceProvider serviceProvider, string serviceKey)
    {
        if (_modelFactories.TryGetValue(serviceKey, out var factory)) return factory;

        throw new InvalidOperationException($"'{serviceKey}' 不是 ServiceKey 的有效值，请确保参数的有效性");
    }

    public static IServiceCollection AddCustomAiClient<TModel>(
        this IServiceCollection services,
        string host,
        string serviceName,
        List<string> models,
        string apiKey)
        where TModel : AiClientBase
    {
        var modelServices = ServiceEnum.GetByName(serviceName);

        var http = new HttpClient() { BaseAddress = new Uri(host) };

        foreach (var modelName in models)
        {
            AiClientBase model = services switch
            {
                var s when s == ServiceEnum.Aliyun => new AliyunAiClient(http) { ServiceName = serviceName, ModelName = modelName, ApiKey = apiKey },
                var s when s == ServiceEnum.Kimi => new KimiAiClient(http) { ServiceName = serviceName, ModelName = modelName, ApiKey = apiKey },
                _ => throw new NotImplementedException($"未实现指定的服务商模型：{serviceName}。")
            };

            _modelFactories[model.ServiceKey] = model;
        }

        // 注册IAiClientFactory
        services.AddSingleton<IAiClientFactory>(serviceProvider => new AiClientFactory(serviceProvider));

        return services;
    }
}
