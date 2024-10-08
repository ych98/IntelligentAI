﻿using System;
using System.Threading.Tasks;
using IntelligentAI.ApiService.AspectInjectors.Events;

namespace IntelligentAI.ApiService.AspectInjectors.Attributes;

public abstract class BaseMethodPointsAspectAttribute : BaseUniversalWrapperAttribute
{
    protected internal sealed override T WrapSync<T>(Func<object[], T> target, object[] args, AspectEventArgs eventArgs)
    {
        OnBefore(eventArgs);

        try
        {
            var result = base.WrapSync(target, args, eventArgs);

            OnAfter(eventArgs);

            return result;
        }
        catch (Exception exception)
        {
            return OnException<T>(eventArgs, exception);
        }
    }

    protected virtual void OnBefore(AspectEventArgs eventArgs)
    {
    }

    protected virtual void OnAfter(AspectEventArgs eventArgs)
    {
    }

    protected internal sealed override async Task<T> WrapAsync<T>(Func<object[], Task<T>> target, object[] args, AspectEventArgs eventArgs)
    {
        await OnBeforeAsync(eventArgs);

        try
        {
            T result = await base.WrapAsync(target, args, eventArgs);

            await OnAfterAsync(eventArgs);

            return result;
        }
        catch (Exception exception)
        {
            return OnException<T>(eventArgs, exception);
        }
    }

    protected virtual Task OnBeforeAsync(AspectEventArgs eventArgs)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnAfterAsync(AspectEventArgs eventArgs)
    {
        return Task.CompletedTask;
    }

    protected virtual T OnException<T>(AspectEventArgs eventArgs, Exception exception)
    {
        throw exception;
    }
}