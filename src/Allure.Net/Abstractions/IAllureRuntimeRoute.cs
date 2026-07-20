using System.Collections.Immutable;

namespace Allure.Abstractions;

public interface IAllureRuntimeRoute
{
    string Id { get; }

    bool MatchCurrentScope { get; }

    bool MatchGlobalScope { get; }

    ImmutableHashSet<string> SuppressedRouteIds { get; }

    IAllureRuntime Runtime { get; }
}
