using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace IntelligentAI.ApiService.Applications;

public class ExceptionHandlerFilter : IExceptionFilter
{
    private readonly IHostEnvironment _hostEnvironment;

    public ExceptionHandlerFilter(IHostEnvironment hostEnvironment) => _hostEnvironment = hostEnvironment;

    public void OnException(ExceptionContext context)
    {
        if (!_hostEnvironment.IsDevelopment())
        {
            if (context.Exception is ApplicationException)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status417ExpectationFailed,
                    ContentType = "application/json;charset=utf-8",
                    Content = context.Exception.Message
                };
                context.ExceptionHandled = true;
                return;
            }

            if (context.Exception is TimeoutException)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status408RequestTimeout,
                    ContentType = "application/json;charset=utf-8",
                    Content = context.Exception.Message
                };
                context.ExceptionHandled = true;
                return;
            }

            if (context.Exception is ArgumentException 
                || context.Exception is ArgumentNullException 
                || context.Exception is ArgumentOutOfRangeException)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    ContentType = "application/json;charset=utf-8",
                    Content = context.Exception.Message
                };
                context.ExceptionHandled = true;
                return;
            }

            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ContentType = "application/json;charset=utf-8",
                Content = context.Exception.ToString()
            };
            context.ExceptionHandled = true;
            return;
        }

        context.Result = new ContentResult
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentType = "application/json;charset=utf-8",
            Content = context.Exception.ToString()
        };
        context.ExceptionHandled = true;
    }
}
