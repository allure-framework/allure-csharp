using System;
using Allure.Abstractions;
using Allure.Internal;

namespace Allure.Runtime;

public static class AllureFrontend
{
    readonly static object monitor = new();

    static bool prepared = false;
    static bool frozen = false;

    private static IAllureTestRuntimeFrontend runtime = SynchronizedInProcessTestRuntime.Instance;

    internal static IAllureTestRuntimeFrontend Runtime
    {
        get
        {
            lock (monitor)
            {
                frozen = true;
                return runtime;
            }
        }
    }

    internal static IAllureInProcessTestApi InProcessApi =>
        Runtime.TestApi.Sync as IAllureInProcessTestApi
            ?? throw new InvalidOperationException(
                $"The in-process test API is not supported by '{Runtime.Name}'."
            );

    public static bool IsAvailable
    {
        get
        {
            lock (monitor)
            {
                return prepared && runtime.IsAllureAvailable;
            }
        }
    }

    public static void PrepareRuntime(IAllureTestRuntimeFrontend runtime)
    {
        if (runtime is null)
        {
            throw new ArgumentNullException(nameof(runtime));
        }

        lock (monitor)
        {
            if (frozen)
            {
                throw new InvalidOperationException(
                    "Frontend preparation failed: the current runtime is already in use."
                );
            }

            if (prepared)
            {
                throw new InvalidOperationException(
                    "Frontend preparation failed: a runtime has already been prepared."
                );
            }

            AllureFrontend.runtime = runtime;
            prepared = true;
        }
    }
}