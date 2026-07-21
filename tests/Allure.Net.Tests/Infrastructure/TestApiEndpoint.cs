using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class TestApiEndpoint(
    IAllureOperations<IAllureStepContext, IAllureFixtureContext>? sync = null,
    IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>? @async = null,
    IAllureParameterSerializer? serializer = null
) : IAllureApiEndpoint
{
    public AllureApiOperations Operations { get; } = new(
        sync ?? InterfaceStub.Create<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>(),
        @async ?? InterfaceStub.Create<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>()
    );

    public IAllureParameterSerializer ParameterSerializer { get; } =
        serializer ?? new TestParameterSerializer();
}
