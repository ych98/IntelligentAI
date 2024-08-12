
using AspectInjector.Broker;
using IntelligentAI.ApiService.AspectInjectors.Aspects;
using IntelligentAI.ApiService.AspectInjectors.Attributes;
using IntelligentAI.ApiService.AspectInjectors.Events;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace IntelligentAI.ApiService.AspectInjectors;

[Injection(typeof(ResponseAspect))]
public class ResponseAttribute : MethodAspectAttribute
{

    #region Sync

    protected override void OnBefore(AspectEventArgs eventArgs)
    {
        Type targetType = typeof(IActionResult); 

        bool isAssignable = targetType.IsAssignableFrom(eventArgs.ReturnType);

        if (!isAssignable) throw new ApplicationException("The return type does not match the expected type - IActionResult, Attribute - [Response] cannot be used!");

        base.OnBefore(eventArgs);
    }

    #endregion

    #region Async

    protected override Task OnBeforeAsync(AspectEventArgs eventArgs)
    {
        Type targetType = typeof(IActionResult);

        bool isAssignable = targetType.IsAssignableFrom(eventArgs.ReturnType);

        if (!isAssignable) throw new ApplicationException("The return type does not match the expected type - IActionResult, Attribute - [Response] cannot be used!");

        return base.OnBeforeAsync(eventArgs);
    }

    #endregion

    #region Exception

    protected override T OnException<T>(AspectEventArgs eventArgs, Exception exception)
    {
        //return ResultModel.Exception(exception);

        return default;
    }

    #endregion

}

[Aspect(Scope.PerInstance)]
public class ResponseAspect : MethodWrapperAspect
{

}