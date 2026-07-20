using System;
using System.Reflection;
using Allure.Internal;
using AspectInjector.Broker;

namespace Allure.Aspects;

[Aspect(Scope.Global)]
public class AllureTearDownAspect
{
    [Advice(Kind.Around, Targets = Target.Method | Target.Constructor)]
    public object? Around(
        [Argument(Source.Name)] string name,
        [Argument(Source.Arguments)] object[] args,
        [Argument(Source.Target)] Func<object[], object> target,
        [Argument(Source.Metadata)] MethodBase metadata,
        [Argument(Source.ReturnType)] Type returnType
    ) =>
        AllureTearDownRouter.Instance.Route(name, metadata, returnType, target!, args);
}