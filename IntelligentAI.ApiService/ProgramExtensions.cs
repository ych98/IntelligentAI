using IntelligentAI.Enumerations;
using IntelligentAI.ApiService.Models;
using IntelligentAI.Extensions;


namespace IntelligentAI.ApiService;

public static class ProgramExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, ModelApis? apis)
    {
        services.AddHttpClient(FanewsApiEnum.KimiService.Name, (client) =>
        {
            client.BaseAddress = new Uri(apis.KimiService);

            client.DefaultRequestHeaders.Add("Accept", "*/*");

            client.Timeout = TimeSpan.FromSeconds(60);

        });

        services.AddHttpClient(FanewsApiEnum.HuoshanService.Name, (client) =>
        {
            client.BaseAddress = new Uri(apis.HuoshanService);

            client.DefaultRequestHeaders.Add("Accept", "*/*");

            client.Timeout = TimeSpan.FromSeconds(60);

        });

        services.AddHttpClient(FanewsApiEnum.AliyunService.Name, (client) =>
        {
            client.BaseAddress = new Uri(apis.AliyunService);

            client.DefaultRequestHeaders.Add("Accept", "*/*");

            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddHttpClient(FanewsApiEnum.BaiduService.Name, (client) =>
        {
            client.BaseAddress = new Uri(apis.BaiduService);

            client.DefaultRequestHeaders.Add("Accept", "*/*");

            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddHttpClient(FanewsApiEnum.AzureService.Name, (client) =>
        {
            client.BaseAddress = new Uri(apis.AzureService);
        });

        return services;
    }

    public static IServiceCollection AddAiModels(this IServiceCollection services, ApiKeys? keys)
    {

        services.AddAiModel(ModelEnum.AliyunModel.Name, ModelEnum.AliyunModel.Description, (option) =>
        {
            option.ConcurrentNumber = 500;
            option.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunMaxModel.Name, ModelEnum.AliyunMaxModel.Description, (option) =>
        {
            option.ConcurrentNumber = 100;
            option.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunPlusModel.Name, ModelEnum.AliyunPlusModel.Description, (option) =>
        {
            option.ConcurrentNumber = 100;
            option.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunTurboModel.Name, ModelEnum.AliyunTurboModel.Description, (option) =>
        {
            option.ConcurrentNumber = 100;
            option.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunInstructModel.Name, ModelEnum.AliyunInstructModel.Description, (option) =>
        {
            option.ConcurrentNumber = 5;
            option.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunBaseInstructModel.Name, ModelEnum.AliyunBaseInstructModel.Description, (option) =>
        {
            option.ConcurrentNumber = 5;
            option.ApiKey = keys.Aliyun;
        });


        services.AddAiModel(ModelEnum.KimiModel.Name, ModelEnum.KimiModel.Description, (option) =>
        {
            option.ConcurrentNumber = 1;
            option.ApiKey = keys.Kimi;
        });

        services.AddAiModel(ModelEnum.BaiduModel.Name, ModelEnum.BaiduModel.Description, (option) =>
        {
            option.ConcurrentNumber = 10;

            option.ApiKey = keys.Baidu;
        });

        services.AddAiModel(ModelEnum.BaiduProModel.Name, ModelEnum.BaiduProModel.Description, (option) =>
        {
            option.ConcurrentNumber = 3;

            option.ApiKey = keys.Baidu;
        });

        services.AddAiModel(ModelEnum.AzureMiniModel.Name, ModelEnum.AzureMiniModel.Description, (option) =>
        {
            option.ApiKey = keys.Azure;
        });

        services.AddAiModel(ModelEnum.GLM3Model.Name, ModelEnum.GLM3Model.Description, (option) =>
        {
            option.ConcurrentNumber = 60;
            option.ApiKey = keys.Huoshan;
        });

        services.AddAiModel(ModelEnum.MistralModel.Name, ModelEnum.MistralModel.Description, (option) =>
        {
            option.ConcurrentNumber = 60;
            option.ApiKey = keys.Huoshan;
        });

        services.AddAiModel(ModelEnum.Llama3Model.Name, ModelEnum.Llama3Model.Description, (option) =>
        {
            option.ConcurrentNumber = 60;
            option.ApiKey = keys.Huoshan;
        });

        services.AddSingleton<IAiModelEventManager, AiModelEventManager>();

        return services;
    }

}
