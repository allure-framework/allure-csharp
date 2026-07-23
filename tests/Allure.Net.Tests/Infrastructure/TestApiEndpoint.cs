using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class TestApiEndpoint(
    IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext>? sync = null,
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

    public AllureOperations Operations { get; } = new AllureOperations(
        sync ?? InterfaceStub.Create<IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext>>(),
        @async ?? InterfaceStub.Create<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>()
    );

    public AllureInProcessOperations InProcessOperations =>
        new(this.inProcessSync, this.inProcessAsync);

    public IAllureParameterSerializer ParameterSerializer { get; } =
        serializer ?? new TestParameterSerializer();
}
