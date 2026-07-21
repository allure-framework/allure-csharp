using Allure.Abstractions;
using Allure.Runtime;

namespace Allure.Internal;

class RoutingAllureApiClient : IAllureApiClient
{
    public string Name => "Routing in-process Allure API client";

    public bool IsAvailableInCurrentScope => AllureBackend.IsAvailableInCurrentScope;

    public bool IsAvailableInGlobalScope => AllureBackend.IsAvailableInGlobalScope;

    public IAllureApiEndpoint? ResolveCurrentScope() =>
        AllureBackend.RuntimeForCurrentScope is { } runtime
            ? new RuntimeApiEndpoint(runtime)
            : null;

    public IAllureApiEndpoint? ResolveGlobalScope() =>
        AllureBackend.RuntimeForGlobalScope is { } runtime
            ? new RuntimeApiEndpoint(runtime)
            : null;

    public static RoutingAllureApiClient Instance { get; } = new();
}