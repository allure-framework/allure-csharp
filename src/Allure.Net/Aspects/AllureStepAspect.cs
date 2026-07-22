using System;
using System.Reflection;
using Allure.Internal;
using AspectInjector.Broker;

namespace Allure.Aspects;

/// <summary>
/// Implements methods annotated with <see cref="AllureStepAttribute"/> as Allure steps.
/// </summary>
[Aspect(Scope.Global)]
public class AllureStepAspect
{
    /// <summary>
    /// Routes an annotated method invocation through the current Allure endpoint.
    /// </summary>
    [Advice(Kind.Around, Targets = Target.Method | Target.Constructor)]
    public object? Around(
        [Argument(Source.Name)] string name,
        [Argument(Source.Arguments)] object[] args,
        [Argument(Source.Target)] Func<object[], object> target,
        [Argument(Source.Metadata)] MethodBase metadata,
        [Argument(Source.ReturnType)] Type returnType
    ) =>
        AllureStepRouter.Instance.Route(name, metadata, returnType, target!, args);
}
