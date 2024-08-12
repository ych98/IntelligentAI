using System;
using System.Threading.Tasks;
using IntelligentAI.ApiService.AspectInjectors.Events;

namespace IntelligentAI.ApiService.AspectInjectors.Attributes
{
    public abstract class BaseUniversalWrapperAttribute : Attribute
    {       
        protected internal virtual T WrapSync<T>(Func<object[], T> target, object[] args, AspectEventArgs eventArgs)
        {
            return target(args);
        }
        protected internal virtual Task<T> WrapAsync<T>(Func<object[], Task<T>> target, object[] args, AspectEventArgs eventArgs)
        {
            return target(args);
        }
    }
}