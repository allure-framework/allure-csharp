using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class TestRuntime(
    string name,
    bool isAvailable = true,
    IAllureInProcessOperations? sync = null,
    IAllureAsyncInProcessOperations? @async = null,
    IAllureParameterSerializer? serializer = null
) : IAllureRuntimeEndpoint
{
    public string Name { get; } = name;

    public bool IsAvailable { get; set; } = isAvailable;

    public IAllureApiOperations Operations { get; } = new TestApiOperations(
        sync ?? InterfaceStub.Create<IAllureInProcessOperations>(),
        @async ?? InterfaceStub.Create<IAllureAsyncInProcessOperations>()
    );

    public IAllureParameterSerializer ParameterSerializer { get; } =
        serializer ?? new TestParameterSerializer(name);
}
