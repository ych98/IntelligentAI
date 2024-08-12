using System;
using AspectInjector.Broker;
using IntelligentAI.ApiService.AspectInjectors.Aspects;

namespace IntelligentAI.ApiService.AspectInjectors.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    [Injection(typeof(PublicMethodWrapperAspect), Inherited = true)]
    public abstract class PublicMethodAspectAttribute: BaseMethodPointsAspectAttribute
    {
    }
}