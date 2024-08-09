using IntelligentAI.Aggregates.AiModels;
using IntelligentAI.Enumerations;
using IntelligentAI.Models;
using IntelligentAI.Records.Fanews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Aggregates.AiFunctionAggregates;

public class AiFunctionModel : IStandardAiFunction, IAggregateRoot
{
    private readonly IAiModelFactory _modelFactory;

    public AiFunctionModel(IAiModelFactory modelFactory) 
    {
        _modelFactory = modelFactory;
    }

    #region 基础功能
    public Task<AiImagesResult> AiGetImageFromImageAsync(string image, string type = nameof(ImageGenerationEnum.New), int modelEnum = 12, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public Task<AiResult> AiGetTextFromImageAsync(string image, int count = 1, string type = nameof(TextFromImageEnum.Content), int modelEnum = 12, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public Task<AiResult> AiGetTextFromTextAsync(string content, int count = 1, string type = nameof(PromptEnum.Summary), int modelEnum = 12, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    #endregion

    public async Task<string> AiGenerateAbstractContentAsync(string content, int modelEnum = 12, CancellationToken cancellation = default)
    {
        AiArguments arguments = new AiArguments(content, PromptEnum.AbstractContent.Name);

        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        return await model.AnswerText(content, arguments.ToDictionary(), cancellation: cancellation);
    }

    public async Task<string> AiGenerateSummaryAsync(string content, int modelEnum = 12, CancellationToken cancellation = default)
    {
        AiArguments arguments = new AiArguments(content, PromptEnum.Summary.Name);

        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        return await model.AnswerText(content, arguments.ToDictionary(),cancellation: cancellation);
    }

    public async Task<string> AiGenerateTitleAsync(string content, int modelEnum = 12, CancellationToken cancellation = default)
    {
        AiArguments arguments = new AiArguments(content, PromptEnum.Title.Name);

        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        return await model.AnswerText(content, arguments.ToDictionary(), cancellation: cancellation);
    }

    public async Task<string> AiGetCoreWordsAsync(string question, int modelEnum = 12, CancellationToken cancellation = default)
    {
        AiArguments arguments = new AiArguments(question, PromptEnum.CoreWord.Name);

        ModelEnum modelInformation = ModelEnum.GetById(modelEnum);

        var model = _modelFactory.CreateModel(modelInformation.Name, modelInformation.Description);

        return await model.AnswerText(question, arguments.ToDictionary(), cancellation: cancellation);
    }

}
