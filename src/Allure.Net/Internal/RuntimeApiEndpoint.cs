using Allure.Abstractions;

namespace Allure.Internal;

public class RuntimeApiEndpoint(
    IAllureRuntime runtime
) : IAllureApiEndpoint
{

    public AllureApiOperations Operations { get; } = new AllureApiOperations(
        runtime.Operations.Sync,
        runtime.Operations.Async
    );

    public IAllureParameterSerializer ParameterSerializer {get; } = runtime.ParameterSerializer;
}
