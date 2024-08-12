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
        return services;
    }

    public static IServiceCollection AddAiModels(this IServiceCollection services, ApiKeys? keys)
    {

        services.AddAiModel(ModelEnum.AliyunModel.Name, ModelEnum.AliyunModel.Description, (model) =>
        {
            model.ConcurrentNumber = 500;
            model.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunMaxModel.Name, ModelEnum.AliyunMaxModel.Description, (model) =>
        {
            model.ConcurrentNumber = 100;
            model.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunPlusModel.Name, ModelEnum.AliyunPlusModel.Description, (model) =>
        {
            model.ConcurrentNumber = 100;
            model.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunTurboModel.Name, ModelEnum.AliyunTurboModel.Description, (model) =>
        {
            model.ConcurrentNumber = 100;
            model.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunInstructModel.Name, ModelEnum.AliyunInstructModel.Description, (model) =>
        {
            model.ConcurrentNumber = 5;
            model.ApiKey = keys.Aliyun;
        });

        services.AddAiModel(ModelEnum.AliyunBaseInstructModel.Name, ModelEnum.AliyunBaseInstructModel.Description, (model) =>
        {
            model.ConcurrentNumber = 5;
            model.ApiKey = keys.Aliyun;
        });


        services.AddAiModel(ModelEnum.KimiModel.Name, ModelEnum.KimiModel.Description, (model) =>
        {
            model.ConcurrentNumber = 1;
            model.ApiKey = keys.Kimi;
        });

        services.AddAiModel(ModelEnum.BaiduModel.Name, ModelEnum.BaiduModel.Description, (model) =>
        {
            model.ConcurrentNumber = 10;

            model.ApiKey = keys.Baidu;
        });

        services.AddAiModel(ModelEnum.BaiduProModel.Name, ModelEnum.BaiduProModel.Description, (model) =>
        {
            model.ConcurrentNumber = 3;

            model.ApiKey = keys.Baidu;
        });


        services.AddAiModel(ModelEnum.GLM3Model.Name, ModelEnum.GLM3Model.Description, (model) =>
        {
            model.ConcurrentNumber = 60;
            model.ApiKey = keys.Huoshan;
        });

        services.AddAiModel(ModelEnum.MistralModel.Name, ModelEnum.MistralModel.Description, (model) =>
        {
            model.ConcurrentNumber = 60;
            model.ApiKey = keys.Huoshan;
        });

        services.AddAiModel(ModelEnum.Llama3Model.Name, ModelEnum.Llama3Model.Description, (model) =>
        {
            model.ConcurrentNumber = 60;
            model.ApiKey = keys.Huoshan;
        });

        services.AddSingleton<IAiModelEventManager, AiModelEventManager>();

        return services;
    }

}
