using System.Collections.Immutable;
using Allure.Abstractions;

namespace Allure.Net.Tests.Routing.Infrastructure;

sealed class RoutingTestRuntime(
    string name,
    IAllureInProcessOperations sync,
    IAllureAsyncInProcessOperations @async,
    IAllureParameterSerializer serializer,
    bool available = true
) : IAllureRuntime
{
    public string Name { get; } = name;

    public bool IsAvailable { get; set; } = available;

    public AllureRuntimeOperations Operations { get; } = new(sync, @async);

    public IAllureParameterSerializer ParameterSerializer { get; } = serializer;
}

sealed class RoutingTestRoute(
    string id,
    IAllureRuntime runtime,
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

    public IAllureRuntime Runtime { get; } = runtime;
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
