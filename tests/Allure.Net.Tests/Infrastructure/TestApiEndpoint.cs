using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class TestApiEndpoint(
    IAllureOperations<IAllureStepContext, IAllureFixtureContext>? sync = null,
    IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>? @async = null,
    IAllureParameterSerializer? serializer = null
) : IAllureInProcessRuntimeEndpoint
{
    readonly IAllureInProcessOperations inProcessSync =
        sync as IAllureInProcessOperations
            ?? InterfaceStub.Create<IAllureInProcessOperations>();

    readonly IAllureAsyncInProcessOperations inProcessAsync =
        @async as IAllureAsyncInProcessOperations
            ?? InterfaceStub.Create<IAllureAsyncInProcessOperations>();

    public string Name => "test endpoint";

    public bool IsAvailable => true;

    public IAllureApiOperations Operations { get; } = new TestApiOperations(
        sync ?? InterfaceStub.Create<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>(),
        @async ?? InterfaceStub.Create<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>()
    );

    public IAllureInProcessApiOperations InProcessOperations =>
        new TestInProcessApiOperations(this.inProcessSync, this.inProcessAsync);

    public IAllureParameterSerializer ParameterSerializer { get; } =
        serializer ?? new TestParameterSerializer();
}

sealed record TestApiOperations(
    IAllureOperations<IAllureStepContext, IAllureFixtureContext> Sync,
    IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> Async
) : IAllureApiOperations;

sealed record TestInProcessApiOperations(
    IAllureInProcessOperations Sync,
    IAllureAsyncInProcessOperations Async
) : IAllureInProcessApiOperations;
