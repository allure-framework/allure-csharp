using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class TestApiEndpoint(
    IAllureSyncOperations<IAllureStepContext, IAllureFixtureContext>? sync = null,
    IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>? @async = null,
    IAllureParameterSerializer? serializer = null
) : IAllureInProcessRuntimeEndpoint
{
    readonly IAllureInProcessSyncOperations inProcessSync =
        sync as IAllureInProcessSyncOperations
            ?? InterfaceStub.Create<IAllureInProcessSyncOperations>();

    readonly IAllureInProcessAsyncOperations inProcessAsync =
        @async as IAllureInProcessAsyncOperations
            ?? InterfaceStub.Create<IAllureInProcessAsyncOperations>();

    public string Name => "test endpoint";

    public bool IsAvailable => true;

    public IAllureOperations Operations { get; } = new TestApiOperations(
        sync ?? InterfaceStub.Create<IAllureSyncOperations<IAllureStepContext, IAllureFixtureContext>>(),
        @async ?? InterfaceStub.Create<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>()
    );

    public IAllureInProcessOperations InProcessOperations =>
        new TestInProcessApiOperations(this.inProcessSync, this.inProcessAsync);

    public IAllureParameterSerializer ParameterSerializer { get; } =
        serializer ?? new TestParameterSerializer();
}

sealed record TestApiOperations(
    IAllureSyncOperations<IAllureStepContext, IAllureFixtureContext> Sync,
    IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> Async
) : IAllureOperations;

sealed record TestInProcessApiOperations(
    IAllureInProcessSyncOperations Sync,
    IAllureInProcessAsyncOperations Async
) : IAllureInProcessOperations;
