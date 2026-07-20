using System.Collections.Immutable;

namespace Allure.Abstractions;

public interface IAllureRuntimeRoute
{
    string Id { get; }

    bool MatchesCurrentScope { get; }

    bool MatchesGlobalScope { get; }

    ImmutableHashSet<string> SuppressedRouteIds { get; }

    IAllureRuntime Runtime { get; }
}
