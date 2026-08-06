using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;
using Allure.Runtime;

namespace Allure.Net.Tests.Runtime;

public class RuntimeRouterIntegrationTests
{
    static readonly AsyncLocal<string?> activeScope = new();

    [Test]
    public async Task SyncFacadeCallReachesCurrentEndpoint()
    {
        var fixture = new RuntimeFixture("current");
        using var scope = ActivateScope();
        using var registration = Install(fixture, current: true);

        AllureApi.SetName("routed name");

        await Assert.That(fixture.Sync.SingleCall.Method.Name).IsEqualTo("SetName");
        await Assert.That(fixture.Sync.SingleCall.Arguments[0]).IsEqualTo("routed name");
    }

    [Test]
    public async Task AsyncFacadeCallReachesCurrentEndpoint()
    {
        var fixture = new RuntimeFixture("current");
        using var scope = ActivateScope();
        using var registration = Install(fixture, current: true);
        using var cancellation = new CancellationTokenSource();

        await AllureApi.SetDescriptionAsync("description", cancellation.Token);

        await Assert.That(fixture.Async.SingleCall.Method.Name).IsEqualTo("SetDescriptionAsync");
        await Assert.That(fixture.Async.SingleCall.Arguments[0]).IsEqualTo("description");
        await Assert.That(fixture.Async.SingleCall.Arguments[1]).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task GlobalFacadeCallPrefersCurrentEndpoint()
    {
        var current = new RuntimeFixture("current");
        var global = new RuntimeFixture("global");
        using var scope = ActivateScope();
        using var currentRegistration = Install(current, current: true);
        using var globalRegistration = Install(global, global: true);

        AllureApi.SetTestName("test");
        AllureApi.AddGlobalError("global error");

        await Assert.That(current.Sync.Calls.Count).IsEqualTo(2);
        await Assert.That(current.Sync.Calls[0].Method.Name).IsEqualTo("SetTestName");
        await Assert.That(current.Sync.Calls[1].Method.Name).IsEqualTo("AddGlobalError");
        await Assert.That(global.Sync.Calls).IsEmpty();
    }

    [Test]
    public async Task ObjectParameterUsesSerializerFromResolvedEndpoint()
    {
        var fixture = new RuntimeFixture("runtime");
        using var scope = ActivateScope();
        using var registration = Install(fixture, current: true);

        AllureApi.AddTestParameterFromObject("argument", 42);

        var parameter = (Parameter)fixture.Sync.SingleCall.Arguments[0]!;
        await Assert.That(parameter.Value).IsEqualTo("runtime:42");
        await Assert.That(fixture.Serializer.InvocationCount).IsEqualTo(1);
    }

    [Test]
    public async Task InProcessFacadeUsesResolvedEndpointOperations()
    {
        var fixture = new RuntimeFixture("runtime");
        fixture.Sync.Handler = (method, arguments) =>
        {
            if (method.Name == "TryReadTestResult")
            {
                arguments[1] = "read value";
                return true;
            }
            return null;
        };
        using var scope = ActivateScope();
        using var registration = Install(fixture, current: true);
        Action<Allure.Model.TestResult> update = result => result.Name = "updated";

        AllureInProcessApi.UpdateTestResult(update);
        var value = AllureInProcessApi.ReadTestResult(result => result.Name);

        await Assert.That(fixture.Sync.Calls[0].Method.Name).IsEqualTo("UpdateTestResult");
        await Assert.That(fixture.Sync.Calls[0].Arguments[0]).IsSameReferenceAs(update);
        await Assert.That(value).IsEqualTo("read value");
    }

    [Test]
    public async Task MissingRouteUsesPublicNoOpContracts()
    {
        using var scope = ActivateScope();

        AllureApi.SetName("ignored");
        await AllureApi.SetNameAsync("ignored");

        AllureInProcessApi.UpdateTestResult(_ => { });
    }

    [Test]
    public async Task DisabledEndpointBehavesAsUnavailable()
    {
        var fixture = new RuntimeFixture("disabled", available: false);
        using var scope = ActivateScope();
        using var registration = Install(fixture, current: true);

        AllureApi.SetName("ignored");

        await Assert.That(fixture.Sync.Calls).IsEmpty();
    }

    [Test]
    public async Task AmbiguityPropagatesThroughPublicFacade()
    {
        var first = new RuntimeFixture("first");
        var second = new RuntimeFixture("second");
        using var scope = ActivateScope();
        using var firstRegistration = Install(first, current: true);
        using var secondRegistration = Install(second, current: true);

        await Assert.That(() => AllureApi.SetName("ambiguous"))
            .Throws<InvalidOperationException>()
            .WithMessageContaining("Unable to route an API call");
    }

    [Test]
    public async Task SuppressionSelectsDominatingEndpointThroughFacade()
    {
        var winner = new RuntimeFixture("winner");
        var loser = new RuntimeFixture("loser");
        using var scope = ActivateScope();
        var loserId = RouteId("loser");
        using var winnerRegistration = Install(
            winner,
            current: true,
            suppressedIds: [loserId]
        );
        using var loserRegistration = Install(loser, current: true, id: loserId);

        AllureApi.SetName("winner");

        await Assert.That(winner.Sync.SingleCall.Arguments[0]).IsEqualTo("winner");
        await Assert.That(loser.Sync.Calls).IsEmpty();
    }

    [Test]
    public async Task DisposedRouteIsNoLongerResolvedByFacade()
    {
        var fixture = new RuntimeFixture("runtime");
        using var scope = ActivateScope();
        var registration = Install(fixture, current: true);
        AllureApi.SetName("before disposal");

        registration.Dispose();
        AllureApi.SetName("after disposal");

        await Assert.That(fixture.Sync.Calls.Count).IsEqualTo(1);
    }

    [Test]
    public async Task ParallelLogicalScopesRemainIsolatedThroughRouter()
    {
        var tasks = Enumerable.Range(0, 16).Select(index => Task.Run(async () =>
        {
            var fixture = new RuntimeFixture($"runtime-{index}");
            using var scope = ActivateScope();
            using var registration = Install(fixture, current: true);

            await Task.Yield();
            AllureApi.SetName($"name-{index}");
            await Task.Yield();
            return fixture.Sync.SingleCall.Arguments[0];
        }));

        var names = await Task.WhenAll(tasks);

        await Assert.That(names).IsEquivalentTo(
            Enumerable.Range(0, 16).Select(index => $"name-{index}")
        );
    }

    static IDisposable Install(
        RuntimeFixture fixture,
        bool current = false,
        bool global = false,
        string? id = null,
        IEnumerable<string>? suppressedIds = null
    )
    {
        var scope = activeScope.Value;
        return AllureRuntimeRouter.Install(new RoutingTestRoute(
            id ?? RouteId(fixture.Runtime.Name),
            fixture.Runtime,
            current: () => current && activeScope.Value == scope,
            global: () => global && activeScope.Value == scope,
            suppressedIds: suppressedIds
        ));
    }

    static IDisposable ActivateScope()
    {
        var previous = activeScope.Value;
        activeScope.Value = Guid.NewGuid().ToString("N");
        return new CallbackDisposable(() => activeScope.Value = previous);
    }

    static string RouteId(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    sealed class RuntimeFixture
    {
        public RuntimeFixture(string name, bool available = true)
        {
            this.Sync = RecordingInterface<IAllureInProcessSyncOperations>.Create();
            this.Async = RecordingInterface<IAllureInProcessAsyncOperations>.Create();
            this.Serializer = new CountingSerializer(name);
            this.Runtime = new RoutingTestRuntime(
                name,
                this.Sync.Instance,
                this.Async.Instance,
                this.Serializer,
                available
            );
        }

        public RecordingInterface<IAllureInProcessSyncOperations> Sync { get; }

        public RecordingInterface<IAllureInProcessAsyncOperations> Async { get; }

        public CountingSerializer Serializer { get; }

        public RoutingTestRuntime Runtime { get; }
    }

    sealed class CallbackDisposable(Action callback) : IDisposable
    {
        Action? callback = callback;

        public void Dispose() => Interlocked.Exchange(ref this.callback, null)?.Invoke();
    }
}
