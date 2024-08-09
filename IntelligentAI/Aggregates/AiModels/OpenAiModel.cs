using IntelligentAI.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAI.Aggregates.AiModels;

public class OpenAiModel : AiModelBase
{
    public OpenAiModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {

    }

    public override Task<string[]> AnswerImages(string input, Dictionary<string, object>? parameters = null, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override IAsyncEnumerable<string> AnswerStream(
        string question, 
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null, 
        CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task<string> AnswerText(
        string question, 
        Dictionary<string, object>? parameters = null,
        Records.Universal.Message[]? messages = null, 
        CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    public override Task<string> AnswerVideo(string input, Dictionary<string, object>? parameters = null, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }

    protected override Dictionary<string, object> GetParameters(string method, Dictionary<string, object>? overrides = null, MissingKeyBehavior missingKeyBehavior = MissingKeyBehavior.Error)
    {
        throw new NotImplementedException();
    }
}
