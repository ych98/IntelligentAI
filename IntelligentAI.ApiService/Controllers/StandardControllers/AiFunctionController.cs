using IntelligentAI.ApiService.Applications;
using Microsoft.AspNetCore.Mvc;

namespace IntelligentAI.ApiService.Controllers.StandardControllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "Standard")]
    [TypeFilter(typeof(ExceptionHandlerFilter))]
    public class AiFunctionController : ControllerBase
    {
        private readonly IStandardAiFunction _aiFunction;

        private readonly ILogger<AiFunctionController> _logger;

        public AiFunctionController(ILogger<AiFunctionController> logger, IStandardAiFunction aiFunction)
        {
            _aiFunction = aiFunction;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> GetCoreWords([FromBody] string input,
            CancellationToken cancellation = default)
        {
            var result = await _aiFunction.AiGetCoreWordsAsync(input, cancellation: cancellation);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetTitle([FromBody] string input,
            CancellationToken cancellation = default)
        {
            var result = await _aiFunction.AiGenerateTitleAsync(input, cancellation: cancellation);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetAbstractContent([FromBody] string input,
            CancellationToken cancellation = default)
        {
            var result = await _aiFunction.AiGenerateAbstractContentAsync(input, cancellation: cancellation);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetSummary([FromBody] string input,
            CancellationToken cancellation = default)
        {
            var result = await _aiFunction.AiGenerateSummaryAsync(input, cancellation: cancellation);

            return Ok(result);
        }

    }
}
