using Allure.Abstractions;
using Allure.Internal;

namespace Allure.Runtime;

/// <summary>
/// Provides process-wide configuration for the test-author-facing Allure API.
/// </summary>
public static class AllureFrontend
{
    readonly static AllureFrontendState state = new (RoutingAllureApiClient.Instance);

    internal static IAllureApiClient Client => state.Client;

    internal static IAllureInProcessOperations InProcessApi =>
        state.InProcessApi;

    /// <summary>
    /// Configures the API client before the frontend is first accessed.
    /// </summary>
    public static void PrepareClient(IAllureApiClient client) =>
        state.PrepareClient(client);
}
