using Allure.Abstractions;
using Allure.Net.Sdk.Tests.Infrastructure;
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
        var runtimeDisposals = 0;
        var (registration, destination) = BuildRegistration(
            args => new SyncDisposableRuntime(args, () =>
            {
                runtimeDisposals++;
                AllureApi.AddGlobalAttachment("during runtime disposal", new byte[] { 1 });
            }),
            registerRoute: true
        );

        AllureApi.AddGlobalAttachment("before disposal", new byte[] { 1 });
        registration.Dispose();
        await registration.DisposeAsync();
        registration.Dispose();

        await Assert.That(runtimeDisposals).IsEqualTo(1);
        await Assert.That(destination.Globals.Count).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldDisposeAsyncOnlyRuntimeAsynchronouslyOnlyOnce()
    {
        var (registration, destination) = BuildRegistration(
            args => new AsyncDisposableRuntime(args),
            registerRoute: true
        );
        var runtime = registration.Runtime;

        AllureApi.AddGlobalAttachment("before disposal", new byte[] { 1 });
        await registration.DisposeAsync();
        await registration.DisposeAsync();
        registration.Dispose();
        AllureApi.AddGlobalAttachment("after disposal", new byte[] { 1 });

        await Assert.That(destination.Globals.Count).IsEqualTo(1);
        await Assert.That(runtime.AsyncDisposeCalls).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldFallBackToSyncDisposalFromDisposeAsync()
    {
        var runtimeDisposals = 0;
        var (registration, _) = BuildRegistration(
            args => new SyncDisposableRuntime(args, () => runtimeDisposals++)
        );

        await registration.DisposeAsync();

        await Assert.That(runtimeDisposals).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldPreferAsyncDisposalWhenRuntimeSupportsBoth()
    {
        var (registration, _) = BuildRegistration(
            args => new DualDisposableRuntime(args)
        );
        var runtime = registration.Runtime;

        await registration.DisposeAsync();

        await Assert.That(runtime.AsyncDisposeCalls).IsEqualTo(1);
        await Assert.That(runtime.SyncDisposeCalls).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldDisposeRuntimeOnlyOnceWhenDisposalCallsRace()
    {
        var (registration, _) = BuildRegistration(
            args => new DualDisposableRuntime(args)
        );
        var runtime = registration.Runtime;

        await Task.WhenAll(
            Task.Run(registration.Dispose),
            registration.DisposeAsync().AsTask()
        );

        await Assert.That(
            runtime.SyncDisposeCalls + runtime.AsyncDisposeCalls
        ).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldRemoveRouteWhenRuntimeDisposalThrows()
    {
        var (registration, destination) = BuildRegistration(
            args => new SyncDisposableRuntime(args, () => throw new TestException()),
            registerRoute: true
        );

        AllureApi.AddGlobalAttachment("before disposal", new byte[] { 1 });
        await Assert.That(registration.Dispose).Throws<TestException>();
        AllureApi.AddGlobalAttachment("after disposal", new byte[] { 1 });

        await Assert.That(destination.Globals.Count).IsEqualTo(1);
    }

    static (
        IAllureRuntimeRegistration<TRuntime> Registration,
        InMemoryResultsDestination Destination
    ) BuildRegistration<TRuntime>(
        RuntimeFactory<TRuntime> runtimeFactory,
        bool registerRoute = false
    ) where TRuntime : IAllureRuntime<AllureConfiguration>
    {
        var destination = new InMemoryResultsDestination();
        var builder = new RegistrationTestRuntimeBuilder<TRuntime>(runtimeFactory);
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseConfiguration(new AllureConfiguration());
            ctx.UseDestination(_ => destination);
            if (registerRoute)
            {
                var isInTestScope = new AsyncLocal<bool>
                {
                    Value = true
                };
                ctx.RegisterInProcessEndpoint(
                    $"registration-test-{Guid.NewGuid():N}",
                    (_, endpoint) =>
                    {
                        endpoint.UseCurrentScopePredicate(_ => false);
                        endpoint.UseGlobalScopePredicate(_ => isInTestScope.Value);
                    }
                );
            }
        });


        return (plan.Build(), destination);
    }

    delegate TRuntime RuntimeFactory<out TRuntime>(RuntimeCreationArguments<AllureConfiguration> args);

    abstract class RuntimeStub(RuntimeCreationArguments<AllureConfiguration> args) :
        AllureRuntime<AllureConfiguration>(args);

    sealed class SyncDisposableRuntime(
        RuntimeCreationArguments<AllureConfiguration> args,
        Action dispose
    ) : RuntimeStub(args), IDisposable
    {
        public void Dispose() => dispose();
    }

    sealed class AsyncDisposableRuntime(RuntimeCreationArguments<AllureConfiguration> args) :
        RuntimeStub(args),
        IAsyncDisposable
    {
        public int AsyncDisposeCalls { get; private set; }

        public ValueTask DisposeAsync()
        {
            this.AsyncDisposeCalls++;
            return ValueTask.CompletedTask;
        }
    }

    sealed class DualDisposableRuntime(RuntimeCreationArguments<AllureConfiguration> args) :
        RuntimeStub(args),
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

    sealed class RegistrationTestRuntimeRegistrationSession<TRuntime>(
        RuntimeFactory<TRuntime> runtimeFactory
    ) :
        AllureRuntimeRegistrationSession<AllureConfiguration, TRuntime>

        where TRuntime : IAllureRuntime<AllureConfiguration>
    {
        protected override TRuntime CreateRuntime(RuntimeCreationArguments<AllureConfiguration> args)
        {
            return runtimeFactory(args);
        }
    }

    sealed class RegistrationTestRuntimeBuilder<TRuntime>(
        RuntimeFactory<TRuntime> runtimeFactory
    ) :
        AllureRuntimeBuilder<
            AllureConfiguration,
            TRuntime
        >("registration-tests", () => new RegistrationTestRuntimeRegistrationSession<TRuntime>(runtimeFactory))

        where TRuntime : IAllureRuntime<AllureConfiguration>;

    sealed class TestException : Exception;
}
