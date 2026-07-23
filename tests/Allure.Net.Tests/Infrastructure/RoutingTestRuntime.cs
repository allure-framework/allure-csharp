using System.Collections.Immutable;
using Allure.Abstractions;
namespace Allure.Net.Tests.Infrastructure;

sealed class RoutingTestRuntime(
    string name,
    IAllureInProcessSyncOperations sync,
    IAllureInProcessAsyncOperations @async,
    IAllureParameterSerializer serializer,
    bool available = true
) : IAllureInProcessRuntimeEndpoint
{
    public string Name { get; } = name;

    public bool IsAvailable { get; set; } = available;

    public IAllureOperations Operations { get; } = new TestApiOperations(sync, @async);

    public IAllureInProcessOperations InProcessOperations { get; } =
        new TestInProcessApiOperations(sync, @async);

    public IAllureParameterSerializer ParameterSerializer { get; } = serializer;
}

sealed class RoutingTestRoute(
    string id,
    IAllureRuntimeEndpoint runtime,
    Func<bool>? current = null,
    Func<bool>? global = null,
    IEnumerable<string>? suppressedIds = null
) : IAllureRuntimeRoute
{
    public string Id { get; } = id;

    public bool MatchesCurrentScope => current?.Invoke() ?? false;

    public bool MatchesGlobalScope => global?.Invoke() ?? false;

    public ImmutableHashSet<string> SuppressedRouteIds { get; } =
        suppressedIds?.ToImmutableHashSet() ?? [];

    public IAllureRuntimeEndpoint Endpoint { get; } = runtime;
}

sealed class CountingSerializer(string prefix) : IAllureParameterSerializer
{
    public int InvocationCount { get; private set; }

    public string Serialize(object? value)
    {
        this.InvocationCount++;
        return $"{prefix}:{value}";
    }
}
