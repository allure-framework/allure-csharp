using Allure.Abstractions;
using Allure.Runtime;

namespace Allure.Internal;

class RoutingAllureApiClient : IAllureApiClient
{
    public string Name => "Routing in-process Allure API client";

    public IAllureApiEndpoint? ResolveCurrentScope() =>
        AllureBackend.ResolveCurrentScope() is { } runtime
            ? new RuntimeApiEndpoint(runtime)
            : null;

    public IAllureApiEndpoint? ResolveGlobalScope() =>
        AllureBackend.ResolveGlobalScope() is { } runtime
            ? new RuntimeApiEndpoint(runtime)
            : null;

    public static RoutingAllureApiClient Instance { get; } = new();
}