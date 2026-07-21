using Allure.Abstractions;
using Allure.Internal;

namespace Allure.Runtime;

public static class AllureFrontend
{
    readonly static AllureFrontendState state = new (RoutingAllureApiClient.Instance);

    internal static IAllureApiClient Client => state.Client;

    internal static IAllureInProcessOperations InProcessApi =>
        state.InProcessApi;

    public static void PrepareClient(IAllureApiClient client) =>
        state.PrepareClient(client);
}