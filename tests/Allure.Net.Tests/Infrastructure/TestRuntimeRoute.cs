using System.Collections.Immutable;
using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class TestRuntimeRoute(
    string id,
    IAllureRuntimeEndpoint runtime,
    Func<bool>? matchesCurrentScope = null,
    Func<bool>? matchesGlobalScope = null,
    IEnumerable<string>? suppressedRouteIds = null
) : IAllureRuntimeRoute
{
    public string Id { get; } = id;

    public bool MatchesCurrentScope => matchesCurrentScope?.Invoke() ?? false;

    public bool MatchesGlobalScope => matchesGlobalScope?.Invoke() ?? false;

    public ImmutableHashSet<string> SuppressedRouteIds { get; } =
        suppressedRouteIds?.ToImmutableHashSet() ?? [];

    public IAllureRuntimeEndpoint Endpoint { get; } = runtime;
}
