using System;
using System.Collections.Immutable;
using Allure.Abstractions;

namespace Allure.Sdk.Internal.Runtime;

class AllureRuntimeRoute(
    string routeId,
    Func<bool> currentScopePredicate,
    Func<bool> globalScopePredicate,
    ImmutableHashSet<string> suppressedRoutes,
    IAllureRuntimeEndpoint endpoint
) : IAllureRuntimeRoute
{
    public string Id => routeId;

    public bool MatchesCurrentScope => currentScopePredicate();

    public bool MatchesGlobalScope => globalScopePredicate();

    public ImmutableHashSet<string> SuppressedRouteIds => suppressedRoutes;

    public IAllureRuntimeEndpoint Endpoint => endpoint;
}
