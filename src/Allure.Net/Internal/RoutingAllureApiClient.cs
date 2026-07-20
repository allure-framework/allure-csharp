using System;
using Allure.Abstractions;
using Allure.Runtime;

namespace Allure.Internal;

class RoutingAllureApiClient : IAllureApiClient
{
    public string Name => "Routing in-process Allure API client";

    public bool IsAvailableInCurrentScope => AllureBackend.IsAvailableInCurrentScope;

    public bool IsAvailableInGlobalScope => AllureBackend.IsAvailableInGlobalScope;

    public AllureApiOperations Operations { get; } = new(
        RoutingAllureOperations.Instance,
        RoutingAllureAsyncOperations.Instance
    );

    public IAllureParameterSerializer ParameterSerializer =>
        AllureBackend.RuntimeForCurrentScope?.ParameterSerializer
            ?? throw new InvalidOperationException(
                "No active Allure runtime was found."
            );

    public static RoutingAllureApiClient Instance { get; } = new();
}