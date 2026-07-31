using System;
using Allure.Abstractions;

namespace Allure.Sdk.Internal.Runtime;

class AllureInProcessRuntimeEndpoint(
    string name,
    Func<bool> availabilityPredicate,
    AllureInProcessOperations operations,
    IAllureParameterSerializer parameterSerializer
) : IAllureInProcessRuntimeEndpoint
{
    public string Name => name;

    public bool IsAvailable => availabilityPredicate();

    public AllureInProcessOperations Operations => operations;

    public IAllureParameterSerializer ParameterSerializer => parameterSerializer;

    public AllureInProcessOperations InProcessOperations => operations;

    AllureOperations IAllureRuntimeEndpoint.Operations { get; } = new(
        operations.Sync,
        operations.Async
    );
}
