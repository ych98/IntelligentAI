﻿using IntelligentAI.Aggregates.AiModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace IntelligentAI.Extensions;

public static class AiModelFactoryServiceCollectionExtensions
{
    private static readonly Dictionary<string, Func<IServiceProvider, AiModelBase>> _modelFactories =
        new Dictionary<string, Func<IServiceProvider, AiModelBase>>();

    public static IServiceCollection AddAiModel(this IServiceCollection services, string serviceName, string modelName, Action<AiModel> setupAction)
    {
        var serviceKey = $"{serviceName}-{modelName}";

        // 注册工厂方法
        _modelFactories[serviceKey] = serviceProvider =>
        {
            // 从服务提供者中获取IHttpClientFactory实例
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            AiModelBase model = serviceName switch
            {
                ModelEnum.FanewsService => new FanewsAiModel(httpClientFactory),
                ModelEnum.AliyunService => new AliyunAiModel(httpClientFactory),
                ModelEnum.OpenAIService => new OpenAiModel(httpClientFactory),
                ModelEnum.GoogleService => new GoogleAiModel(httpClientFactory),
                ModelEnum.KimiService => new KimiAiModel(httpClientFactory),
                ModelEnum.HuoshanService => new HuoshanAiModel(httpClientFactory),
                ModelEnum.BaiduService => new BaiduAiModel(httpClientFactory),
                _ => throw new NotImplementedException($"未实现指定的服务名称模型适配器：{serviceName}。")
            };

            var modelOptions = new AiModel(httpClientFactory);
            setupAction(modelOptions);

            model.ServiceName = serviceName;
            model.ModelName = modelName;
            model.ApiKey = modelOptions.ApiKey;
            model.ConcurrentNumber = modelOptions.ConcurrentNumber;

            return model;
        };

        // 注册IAiModelFactory
        services.AddSingleton<IAiModelFactory>(serviceProvider => new AiModelFactory(serviceProvider));

        return services;
    }

    public static AiModelBase GetModel(IServiceProvider serviceProvider, string serviceKey)
    {
        if (_modelFactories.TryGetValue(serviceKey, out var factory)) return factory(serviceProvider);

        throw new KeyNotFoundException($"未找到指定的模型实例：{serviceKey}。");
    }
}

