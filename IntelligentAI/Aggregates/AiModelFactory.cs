using IntelligentAI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IntelligentAI.Aggregates;

public class AiModelFactory : IAiModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public AiModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AiModelBase CreateModel(string serviceName, string modelName)
    {
        var serviceKey = $"{serviceName}-{modelName}";

        return AiModelFactoryServiceCollectionExtensions.GetModel(_serviceProvider, serviceKey);
    }
}
