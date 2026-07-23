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
        IAllureSyncOperations_TStepContext_TFixtureContext_Mock<IAllureSyncStepContext, IAllureSyncFixtureContext>,
        IAllureAsyncOperations_TStepContext_TFixtureContext_Mock<IAllureAsyncStepContext, IAllureAsyncFixtureContext>
    > InstallEndpoint(
        InstallationScope scope = InstallationScope.CurrentAndGlobal,
        IAllureParameterSerializer? serializer = null
    )
    {
        var sync = IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext>.Mock();
        var @async = IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>.Mock();

        var operations = new AllureOperations(sync, @async);

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

        var operations = new AllureOperations(sync, @async);
        var inProcessOperations = new AllureInProcessOperations(sync, @async);

        var currentEndpoint = scope is InstallationScope.Current or InstallationScope.CurrentAndGlobal
            ? IAllureInProcessRuntimeEndpoint.Mock()
            : null;
        currentEndpoint?.Operations.Returns(operations);
        currentEndpoint?.InProcessOperations.Returns(inProcessOperations);
        currentEndpoint?.IsAvailable.Returns(true);
        currentEndpoint?.ParameterSerializer.Returns(
            serializer ?? new TestParameterSerializer()
        );

        var globalEndpoint = scope is InstallationScope.Global or InstallationScope.CurrentAndGlobal
            ? IAllureInProcessRuntimeEndpoint.Mock()
            : null;
        globalEndpoint?.Operations.Returns(operations);
        globalEndpoint?.InProcessOperations.Returns(inProcessOperations);
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
}
