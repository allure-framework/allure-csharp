using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure scope has stopped.
/// </summary>
public sealed class AllureScopeStopMessage(
    CorrelationUid correlationUid,
    ScopeExecutionStateUid scopeUid
) :
    AllureModelRemoveMessage(
        "Allure scope stop",
        "This message reports that an Allure test scope has stopped.",
        correlationUid,
        scopeUid
    )
{
    /// <summary>
    /// Gets the scope context identifier.
    /// </summary>
    public ScopeExecutionStateUid ScopeUid { get; } = scopeUid;

    /// <inheritdoc />
    public override void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime)
    {
        var scope = allureRuntime.LifecycleApi.StopTestScope();
        allureRuntime.ResultsDestination.WriteContainer(scope);
    }
}
