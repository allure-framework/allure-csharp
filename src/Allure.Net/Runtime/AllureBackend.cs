using System;
using System.Collections.Immutable;
using System.Linq;
using Allure.Abstractions;

namespace Allure.Runtime;

public static class AllureBackend
{
    readonly static object monitor = new();

    static readonly ImmutableArray<IAllureBackendDispatcher>.Builder dispatchersBuilder =
        ImmutableArray.CreateBuilder<IAllureBackendDispatcher>();

    static ImmutableArray<IAllureBackendDispatcher>? dispatchers;

    static ImmutableArray<IAllureBackendDispatcher> Dispatchers
    {
        get
        {
            lock(monitor)
            {
                frozen = true;
                return dispatchers ??= dispatchersBuilder.ToImmutable();
            }
        }
    }

    static bool frozen = false;

    internal static IAllureTestRuntimeBackend? CurrentBackend
    {
        get
        {
            var currentDispatchers =
                Dispatchers.Where(static (d) => d.IsCurrent && d.Backend.IsAllureAvailable)
                .ToImmutableArray();

            if (currentDispatchers.Length > 1)
            {
                var runtimeNames = string.Join(
                    ", ",
                    currentDispatchers.Select(static (d) => d.Name)
                );

                throw new InvalidOperationException(
                    $"Unable to route an API call to an Allure runtime: "
                        + $"more than one runtime is defined in the current scope: {runtimeNames}. "
                        + "These runtimes are most probably incompatible with each other."
                );
            }

            return currentDispatchers.IsEmpty
                ? null
                : currentDispatchers[0].Backend;
        }
    }

    public static bool IsAvailable =>
        Dispatchers.Any(static (d) => d.IsCurrent && d.Backend.IsAllureAvailable);

    public static void Install(IAllureBackendDispatcher backendDispatcher)
    {
        if (backendDispatcher is null)
        {
            throw new ArgumentNullException(nameof(backendDispatcher));
        }

        lock (monitor)
        {
            if (frozen)
            {
                throw new InvalidOperationException(
                    "Backend preparation failed: the current runtime is already in use."
                );
            }

            dispatchersBuilder.Add(backendDispatcher);
        }
    }
}