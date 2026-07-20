using System;
using Allure.Abstractions;
using Allure.Runtime;

namespace Allure.Internal;

class SynchronizedInProcessTestRuntime : IAllureTestRuntimeFrontend
{
    public string Name => "Synchronized in-process Allure test runtime";

    public bool IsAllureAvailable => AllureBackend.IsAvailable;

    public AllureFrontendTestApi TestApi { get; } = new(
        SynchronizedInProcessTestApi.Instance,
        SynchronizedInProcessTestApiAsync.Instance
    );

    public IAllureParameterSerializer ParameterSerializer =>
        AllureBackend.CurrentBackend?.ParameterSerializer
            ?? throw new InvalidOperationException(
                "No active Allure runtime was found."
            );

    public static SynchronizedInProcessTestRuntime Instance { get; } = new();
}