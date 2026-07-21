using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Allure.Abstractions;
using Allure.Internal;

namespace Allure.Runtime;

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

    public IAllureRuntime? ResolveCurrentScope() =>
        this.GetRuntime(static (r) => r.MatchesCurrentScope);

    public IAllureRuntime? ResolveGlobalScope() =>
        this.GetRuntime(static (r) => r.MatchesGlobalScope);

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

    IAllureRuntime? GetRuntime(Func<IAllureRuntimeRoute, bool> predicate) =>
        this.MatchRuntime(predicate) switch
        {
            MatchSuccess { Runtime: var runtime } => runtime,

            MultipleMatches { Ids: var ids } =>
                throw CreateMultipleMatchesException(ids),

            _ => null,
        };

    static InvalidOperationException CreateMultipleMatchesException(
        IEnumerable<string> ids
    ) =>
        new (
            $"Unable to route an API call to an Allure runtime: "
                + $"more than one runtime matched the requested Allure scope: "
                + $"{string.Join(", ", ids)}. "
                + "Configure the runtime suppression rules and try again."
        );

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
            return EvaluateAvailability(candidates[0].Runtime);
        }

        var winners = FindDominatingRoutes(candidates);

        if (winners.Length != 1)
        {
            return RuntimeMatchResult.Multiple(candidates);
        }

        return EvaluateAvailability(winners[0].Runtime);

        static RuntimeMatchResult EvaluateAvailability(IAllureRuntime runtime) =>
            runtime.IsAvailable
                ? RuntimeMatchResult.Success(runtime)
                : RuntimeMatchResult.Disabled(runtime);

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
        public static MatchSuccess Success(IAllureRuntime runtime) => new(runtime);

        public static RuntimeDisabled Disabled(IAllureRuntime runtime) => new(runtime);

        public static MultipleMatches Multiple(IEnumerable<IAllureRuntimeRoute> matches) => new(
            [..matches.Select(static (d) => $"{d.Runtime.Name} ({d.Id})")]
        );

        public static NoMatch NoMatch { get; } = new();

    }

    sealed record class MatchSuccess(IAllureRuntime Runtime) : RuntimeMatchResult;

    sealed record class RuntimeDisabled(IAllureRuntime Runtime) : RuntimeMatchResult;

    sealed record class NoMatch() : RuntimeMatchResult;

    sealed record class MultipleMatches(ImmutableArray<string> Ids) : RuntimeMatchResult;
}