using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class AllureApiTestsBase
{
    public enum InstallationScope
    {
        Current,
        Global,
        CurrentAndGlobal
    }

    public static EndpointMocks<
        IAllureSyncOperations_TStepContext_TFixtureContext_Mock<IAllureStepContext, IAllureFixtureContext>,
        IAllureAsyncOperations_TStepContext_TFixtureContext_Mock<IAllureAsyncStepContext, IAllureAsyncFixtureContext>
    > InstallEndpoint(
        InstallationScope scope = InstallationScope.CurrentAndGlobal,
        IAllureParameterSerializer? serializer = null
    )
    {
        var sync = IAllureSyncOperations<IAllureStepContext, IAllureFixtureContext>.Mock();
        var @async = IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>.Mock();

        var operations = new AllureApiOperations(sync, @async);

        var currentEndpoint = scope is InstallationScope.Current or InstallationScope.CurrentAndGlobal
            ? IAllureRuntimeEndpoint.Mock()
            : null;
        currentEndpoint?.Operations.Returns(operations);
        currentEndpoint?.IsAvailable.Returns(true);
        currentEndpoint?.ParameterSerializer.Returns(
            serializer ?? new TestParameterSerializer()
        );

        var globalEndpoint = scope is InstallationScope.Global or InstallationScope.CurrentAndGlobal
            ? IAllureRuntimeEndpoint.Mock()
            : null;
        globalEndpoint?.Operations.Returns(operations);
        globalEndpoint?.IsAvailable.Returns(true);
        globalEndpoint?.ParameterSerializer.Returns(
            serializer ?? new TestParameterSerializer()
        );

        return EndpointMocks.Create(
            sync,
            @async,
            FacadeTestEnvironment.Use(current: currentEndpoint, global: globalEndpoint)
        );
    }

    public static IDisposable InstallNoEndpoint() =>
        FacadeTestEnvironment.Use();

    public static EndpointMocks<IAllureInProcessSyncOperationsMock, IAllureInProcessAsyncOperationsMock> InstallInProcessEndpoint(
        InstallationScope scope = InstallationScope.CurrentAndGlobal,
        IAllureParameterSerializer? serializer = null
    )
    {
        var sync = IAllureInProcessSyncOperations.Mock();
        var @async = IAllureInProcessAsyncOperations.Mock();

        var operations = new AllureInProcessApiOperations(sync, @async);

        var currentEndpoint = scope is InstallationScope.Current or InstallationScope.CurrentAndGlobal
            ? IAllureInProcessRuntimeEndpoint.Mock()
            : null;
        currentEndpoint?.Operations.Returns(operations);
        currentEndpoint?.InProcessOperations.Returns(operations);
        currentEndpoint?.IsAvailable.Returns(true);
        currentEndpoint?.ParameterSerializer.Returns(
            serializer ?? new TestParameterSerializer()
        );

        var globalEndpoint = scope is InstallationScope.Global or InstallationScope.CurrentAndGlobal
            ? IAllureInProcessRuntimeEndpoint.Mock()
            : null;
        globalEndpoint?.Operations.Returns(operations);
        globalEndpoint?.InProcessOperations.Returns(operations);
        globalEndpoint?.IsAvailable.Returns(true);
        globalEndpoint?.ParameterSerializer.Returns(
            serializer ?? new TestParameterSerializer()
        );

        return EndpointMocks.Create(
            sync,
            @async,
            FacadeTestEnvironment.Use(current: currentEndpoint, global: globalEndpoint)
        );
    }

    public sealed class EndpointMocks<TSyncApi, TAsyncApi>(
        TSyncApi sync,
        TAsyncApi @async,
        IDisposable registration
    ) : IDisposable
    {
        public TSyncApi SyncApi { get; } = sync;

        public TAsyncApi AsyncApi { get; } = @async;

        public void Dispose()
        {
            registration.Dispose();
        }
    }

    public static class EndpointMocks
    {
        public static EndpointMocks<TSyncApi, TAsyncApi> Create<TSyncApi, TAsyncApi>(
            TSyncApi sync,
            TAsyncApi @async,
            IDisposable registration
        ) =>
            new(sync, @async, registration);
    };

    public class AllureApiOperations(
        IAllureSyncOperations<IAllureStepContext, IAllureFixtureContext> sync,
        IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> @async
    ) : IAllureOperations
    {
        public IAllureSyncOperations<IAllureStepContext, IAllureFixtureContext> Sync => sync;

        public IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> Async => @async;
    }

    public class AllureInProcessApiOperations(
        IAllureInProcessSyncOperations sync,
        IAllureInProcessAsyncOperations @async
    ) : IAllureInProcessOperations, IAllureOperations
    {
        public IAllureInProcessSyncOperations Sync => sync;

        public IAllureInProcessAsyncOperations Async => @async;

        IAllureSyncOperations<IAllureStepContext, IAllureFixtureContext> IAllureOperations.Sync => Sync;

        IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> IAllureOperations.Async => Async;
    }
}
