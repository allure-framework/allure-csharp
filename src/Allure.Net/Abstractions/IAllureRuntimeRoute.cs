using System.Collections.Immutable;

namespace Allure.Abstractions;

/// <summary>
/// Defines how API calls are matched and routed to an Allure runtime endpoint.
/// </summary>
public interface IAllureRuntimeRoute
{
    /// <summary>
    /// Gets the route identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets whether the route matches the current test or fixture scope.
    /// </summary>
    bool MatchesCurrentScope { get; }

    /// <summary>
    /// Gets whether the route can accept global result data when no
    /// current-scope endpoint is available.
    /// </summary>
    bool MatchesGlobalScope { get; }

    /// <summary>
    /// Gets route identifiers suppressed when this route matches.
    /// </summary>
    ImmutableHashSet<string> SuppressedRouteIds { get; }

    /// <summary>
    /// Gets the runtime endpoint served by this route.
    /// </summary>
    IAllureRuntimeEndpoint Endpoint { get; }
}
