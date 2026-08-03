using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Net.Sdk.Tests.Registration;

public class AllureRuntimeRegistrationTests
{
    [Test]
    public async Task ShouldDisposeRouteBeforeRuntimeOnlyOnce()
    {
        var events = new List<string>();
        var runtime = new SyncDisposableRuntime(
            () => events.Add("runtime")
        );
        var route = new CallbackDisposable(
            () => events.Add("route")
        );
        var registration = new AllureRuntimeRegistration<SyncDisposableRuntime>(
            runtime,
            route
        );

        registration.Dispose();
        await registration.DisposeAsync();

        await Assert.That(events.Count).IsEqualTo(2);
        await Assert.That(events[0]).IsEqualTo("route");
        await Assert.That(events[1]).IsEqualTo("runtime");

        registration.Dispose();
        await Assert.That(events.Count).IsEqualTo(2);
    }

    [Test]
    public async Task ShouldDisposeAsyncOnlyRuntimeAsynchronouslyOnlyOnce()
    {
        var routeDisposals = 0;
        var runtime = new AsyncDisposableRuntime();
        var registration = new AllureRuntimeRegistration<AsyncDisposableRuntime>(
            runtime,
            new CallbackDisposable(() => routeDisposals++)
        );

        await registration.DisposeAsync();
        await registration.DisposeAsync();
        registration.Dispose();

        await Assert.That(routeDisposals).IsEqualTo(1);
        await Assert.That(runtime.AsyncDisposeCalls).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldFallBackToSyncDisposalFromDisposeAsync()
    {
        var runtimeDisposals = 0;
        var runtime = new SyncDisposableRuntime(
            () => runtimeDisposals++
        );
        var registration = new AllureRuntimeRegistration<SyncDisposableRuntime>(
            runtime,
            routeRegistration: null
        );

        await registration.DisposeAsync();

        await Assert.That(runtimeDisposals).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldPreferAsyncDisposalWhenRuntimeSupportsBoth()
    {
        var runtime = new DualDisposableRuntime();
        var registration = new AllureRuntimeRegistration<DualDisposableRuntime>(
            runtime,
            routeRegistration: null
        );

        await registration.DisposeAsync();

        await Assert.That(runtime.AsyncDisposeCalls).IsEqualTo(1);
        await Assert.That(runtime.SyncDisposeCalls).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldDisposeRuntimeOnlyOnceWhenDisposalCallsRace()
    {
        var routeDisposals = 0;
        var runtime = new DualDisposableRuntime();
        var registration = new AllureRuntimeRegistration<DualDisposableRuntime>(
            runtime,
            new CallbackDisposable(() => routeDisposals++)
        );

        await Task.WhenAll(
            Task.Run(registration.Dispose),
            registration.DisposeAsync().AsTask()
        );

        await Assert.That(routeDisposals).IsEqualTo(1);
        await Assert.That(
            runtime.SyncDisposeCalls + runtime.AsyncDisposeCalls
        ).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldRemoveRouteWhenRuntimeDisposalThrows()
    {
        var events = new List<string>();
        var runtime = new SyncDisposableRuntime(() =>
        {
            events.Add("runtime");
            throw new TestException();
        });
        var registration = new AllureRuntimeRegistration<SyncDisposableRuntime>(
            runtime,
            new CallbackDisposable(() => events.Add("route"))
        );

        await Assert.That(registration.Dispose).Throws<TestException>();

        await Assert.That(events.Count).IsEqualTo(2);
        await Assert.That(events[0]).IsEqualTo("route");
        await Assert.That(events[1]).IsEqualTo("runtime");
    }

    abstract class RuntimeStub : IAllureRuntime
    {
        public AllureConfiguration Configuration =>
            throw new NotSupportedException();

        public IAllureExecutionContext ContextApi =>
            throw new NotSupportedException();

        public IAllureLifecycleApi LifecycleApi =>
            throw new NotSupportedException();

        public IAllureModelApi ModelApi =>
            throw new NotSupportedException();

        public IAllureResultsDestination ResultsDestination =>
            throw new NotSupportedException();

        public IAllureParameterSerializer ParameterSerializer =>
            throw new NotSupportedException();
    }

    sealed class SyncDisposableRuntime(Action dispose) :
        RuntimeStub,
        IDisposable
    {
        public void Dispose() => dispose();
    }

    sealed class AsyncDisposableRuntime : RuntimeStub, IAsyncDisposable
    {
        public int AsyncDisposeCalls { get; private set; }

        public ValueTask DisposeAsync()
        {
            this.AsyncDisposeCalls++;
            return ValueTask.CompletedTask;
        }
    }

    sealed class DualDisposableRuntime :
        RuntimeStub,
        IDisposable,
        IAsyncDisposable
    {
        int asyncDisposeCalls;
        int syncDisposeCalls;

        public int AsyncDisposeCalls =>
            Volatile.Read(ref this.asyncDisposeCalls);

        public int SyncDisposeCalls =>
            Volatile.Read(ref this.syncDisposeCalls);

        public void Dispose()
        {
            Interlocked.Increment(ref this.syncDisposeCalls);
        }

        public ValueTask DisposeAsync()
        {
            Interlocked.Increment(ref this.asyncDisposeCalls);
            return ValueTask.CompletedTask;
        }
    }

    sealed class CallbackDisposable(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }

    sealed class TestException : Exception;
}
