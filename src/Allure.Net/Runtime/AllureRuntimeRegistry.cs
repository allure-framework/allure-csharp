using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Allure.Abstractions;
using Allure.Internal;

namespace Allure.Runtime;

/// <summary>
/// Stores runtime routes and resolves the runtime matching an API scope.
/// </summary>
public sealed class AllureRuntimeRegistry
{
    readonly object monitor = new();

    readonly List<IAllureRuntimeRoute> routes = [];

    ImmutableArray<IAllureRuntimeRoute> Routes
    {
        get
        {
            lock (monitor)
            {
                return [.. routes];
            }
        }
    }

    /// <summary>
    /// Resolves the runtime endpoint matching the current test or fixture scope.
    /// </summary>
    public IAllureRuntimeEndpoint? ResolveCurrentScope() =>
        this.MatchRuntime(static (r) => r.MatchesCurrentScope) switch
        {
            MatchSuccess { Route: var route } => route.Endpoint,

            MultipleMatches { Routes: var routes } =>
                throw CreateMultipleMatchesException(routes),

            _ => null,
        };

    /// <summary>
    /// Resolves an available runtime endpoint whose route matches the global scope.
    /// When multiple routes match and suppression does not select a single route,
    /// prefers the sole route that also matches the current scope.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Global-scope routing remains ambiguous after applying route suppression and
    /// current-scope preference.
    /// </exception>
    public IAllureRuntimeEndpoint? ResolveGlobalScope() =>
        this.MatchRuntime(static (r) => r.MatchesGlobalScope) switch
        {
            MatchSuccess { Route: var route } => route.Endpoint,

            MultipleMatches { Routes: var routes } =>
                routes.Where(static (r) => r.MatchesCurrentScope)
                    .ToImmutableArray() is { Length: 1 } currentScopeMatches
                        ? currentScopeMatches[0].Endpoint is { IsAvailable: true } endpoint
                            ? endpoint
                            : null
                        : throw CreateMultipleMatchesException(routes),

            _ => null,
        };

    /// <summary>
    /// Installs a runtime route.
    /// The installation lasts until the returned registration is disposed.
    /// </summary>
    public IDisposable Install(IAllureRuntimeRoute route)
    {
        if (route is null)
        {
            throw new ArgumentNullException(nameof(route));
        }

        lock (monitor)
        {
            if (routes.Any((r) => ReferenceEquals(r, route)))
            {
                throw new InvalidOperationException(
                    $"Cannot install an Allure runtime route {route.Id}: "
                        + "this route is already installed"
                );
            }

            routes.Add(route);
        }

        return new RuntimeRegistrationHandle(() =>
        {
            lock (monitor)
            {
                routes.Remove(route);
            }
        });
    }

    static InvalidOperationException CreateMultipleMatchesException(
        IEnumerable<IAllureRuntimeRoute> routes
    )
    {
        IEnumerable<string> ids = [.. routes.Select(static (r) => $"{r.Endpoint.Name} ({r.Id})")];
        return new (
            $"Unable to route an API call to an Allure runtime: "
                + $"more than one routes matched the requested Allure scope: "
                + $"{string.Join(", ", ids)}. "
                + "Configure the route suppression rules and try again."
        );
    }

    RuntimeMatchResult MatchRuntime(Func<IAllureRuntimeRoute, bool> predicate)
    {
        var candidates =
                this.Routes.Where(predicate)
                .ToImmutableArray();

        if (candidates.Length == 0)
        {
            return RuntimeMatchResult.NoMatch;
        }

        if (candidates.Length == 1)
        {
            return EvaluateAvailability(candidates[0]);
        }

        var winners = FindDominatingRoutes(candidates);

        if (winners.Length != 1)
        {
            return RuntimeMatchResult.Multiple(candidates);
        }

        return EvaluateAvailability(winners[0]);

        static RuntimeMatchResult EvaluateAvailability(IAllureRuntimeRoute route) =>
            route.Endpoint.IsAvailable
                ? RuntimeMatchResult.Success(route)
                : RuntimeMatchResult.Disabled(route);

        static ImmutableArray<IAllureRuntimeRoute> FindDominatingRoutes(
            IEnumerable<IAllureRuntimeRoute> matches
        ) => [
            .. matches.Where(
                (candidate) => matches.All(
                    (other) => ReferenceEquals(other, candidate)
                        || candidate.SuppressedRouteIds.Contains(other.Id)
                )
            )
        ];
    }

    record class RuntimeMatchResult
    {
        public static MatchSuccess Success(IAllureRuntimeRoute route) => new(route);

        public static RuntimeDisabled Disabled(IAllureRuntimeRoute route) => new(route);

        public static MultipleMatches Multiple(IEnumerable<IAllureRuntimeRoute> routes) => new(
            [..routes]
        );

        public static NoMatch NoMatch { get; } = new();

    }

    sealed record class MatchSuccess(IAllureRuntimeRoute Route) : RuntimeMatchResult;

    sealed record class RuntimeDisabled(IAllureRuntimeRoute Route) : RuntimeMatchResult;

    sealed record class NoMatch() : RuntimeMatchResult;

    sealed record class MultipleMatches(ImmutableArray<IAllureRuntimeRoute> Routes) : RuntimeMatchResult;
}
