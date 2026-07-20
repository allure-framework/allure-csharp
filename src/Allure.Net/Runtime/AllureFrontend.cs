using System;
using Allure.Abstractions;
using Allure.Internal;

namespace Allure.Runtime;

public static class AllureFrontend
{
    readonly static object monitor = new();

    static bool provided = false;
    static bool frozen = false;

    private static IAllureApiClient client = RoutingAllureApiClient.Instance;

    internal static IAllureApiClient Client
    {
        get
        {
            lock (monitor)
            {
                frozen = true;
                return client;
            }
        }
    }

    internal static IAllureInProcessOperations InProcessApi =>
        Client.Operations.Sync as IAllureInProcessOperations
            ?? throw new InvalidOperationException(
                $"The in-process test API is not supported by '{Client.Name}'."
            );

    public static bool IsAvailable
    {
        get
        {
            lock (monitor)
            {
                frozen = true;
                return client.IsAllureAvailable;
            }
        }
    }

    public static void PrepareClient(IAllureApiClient client)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        lock (monitor)
        {
            if (frozen)
            {
                throw new InvalidOperationException(
                    "Allure API client preparation failed: the current client is already in use."
                );
            }

            if (provided)
            {
                throw new InvalidOperationException(
                    "Allure API client preparation failed: a client has already been prepared."
                );
            }

            AllureFrontend.client = client;
            provided = true;
        }
    }
}