using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Allure.Abstractions;

namespace Allure.Runtime;

public static class AllureBackend
{
    readonly static object monitor = new();

    readonly static List<IAllureRuntimeRoute> dispatchers = [];

    static ImmutableArray<IAllureRuntimeRoute> Dispatchers
    {
        get
        {
            lock (monitor)
            {
                return [.. dispatchers];
            }
        }
    }

    internal static IAllureRuntime? CurrentBackend => MatchBackend() switch
    {
        MatchSuccess { Backend: var backend } => backend,

        MultipleMatches { Ids: var ids } =>
            throw new InvalidOperationException(
                $"Unable to route an API call to an Allure runtime: "
                    + $"more than one runtime was selected in the current scope: "
                    + $"{string.Join(", ", ids)}. "
                    + "Configure the runtime suppression rules and try again."
            ),

        BackendDisabled { Backend: var backend } =>
            throw new InvalidOperationException(
                $"Unable to route an API call to an Allure runtime: "
                    + $"the selected runtime '{backend.Name}' is disabled."
            ),

        _ => null,
    };

    public static bool IsAvailable => MatchBackend() is MatchSuccess;

    public static void Install(IAllureRuntimeRoute backendDispatcher)
    {
        if (backendDispatcher is null)
        {
            throw new ArgumentNullException(nameof(backendDispatcher));
        }

        lock (monitor)
        {
            dispatchers.Add(backendDispatcher);
        }
    }

    public static void Remove(IAllureRuntimeRoute backendDispatcher)
    {
        if (backendDispatcher is null)
        {
            throw new ArgumentNullException(nameof(backendDispatcher));
        }

        lock (monitor)
        {
            dispatchers.Remove(backendDispatcher);
        }
    }

    static IBackendMatchResult MatchBackend()
    {
        var currentDispatchers =
                Dispatchers.Where(static (d) => d.IsCurrent)
                .ToImmutableArray();

            if (currentDispatchers.Length > 1)
            {
                currentDispatchers = [.. currentDispatchers.Where(
                    (candidate) => currentDispatchers.All(
                        (other) => ReferenceEquals(other, candidate)
                            || candidate.SupressRuntimes.Contains(other.Id)
                    )
                )];
            }

            if (currentDispatchers.Length > 1)
            {
                return new MultipleMatches(
                    [.. currentDispatchers.Select(static (d) => d.Id)]
                );
            }

            if (currentDispatchers.Length == 0)
            {
                return new NoMatch();
            }

            var runtime = currentDispatchers[0].Backend;
            if (!runtime.IsAllureAvailable)
            {
                return new BackendDisabled(runtime);
            }

            return new MatchSuccess(runtime);
    }

    interface IBackendMatchResult;

    sealed record class MatchSuccess(IAllureRuntime Backend) : IBackendMatchResult;

    sealed record class BackendDisabled(IAllureRuntime Backend) : IBackendMatchResult;

    sealed record class NoMatch() : IBackendMatchResult;

    sealed record class MultipleMatches(ImmutableArray<string> Ids) : IBackendMatchResult;
}