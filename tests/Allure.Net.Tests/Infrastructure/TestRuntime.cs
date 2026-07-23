using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class TestRuntime(
    string name,
    bool isAvailable = true,
    IAllureInProcessSyncOperations? sync = null,
    IAllureInProcessAsyncOperations? @async = null,
    IAllureParameterSerializer? serializer = null
) : IAllureRuntimeEndpoint
{
    public string Name { get; } = name;

    public bool IsAvailable { get; set; } = isAvailable;

    public IAllureOperations Operations { get; } = new TestApiOperations(
        sync ?? InterfaceStub.Create<IAllureInProcessSyncOperations>(),
        @async ?? InterfaceStub.Create<IAllureInProcessAsyncOperations>()
    );

    public IAllureParameterSerializer ParameterSerializer { get; } =
        serializer ?? new TestParameterSerializer(name);
}
