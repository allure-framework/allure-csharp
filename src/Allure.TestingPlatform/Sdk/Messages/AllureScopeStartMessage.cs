using Allure.Sdk.Functions;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure scope has started.
/// </summary>
public sealed class AllureScopeStartMessage(
    CorrelationUid correlationUid,
    ScopeExecutionStateUid scopeUid,
    ScopeExecutionStateUid? parentScopeUid = null
) :
    AllureModelCreateMessage(
        "Allure scope start",
        "This message reports that an Allure test scope has started.",
        correlationUid,
        scopeUid,
        parentScopeUid
    )
{
    /// <summary>
    /// Gets the scope context identifier.
    /// </summary>
    public ScopeExecutionStateUid ScopeUid { get; } = scopeUid;

    /// <summary>
    /// Gets the parent scope context identifier, if one exists.
    /// </summary>
    public ScopeExecutionStateUid? ParentScopeUid { get; } = parentScopeUid;

    /// <inheritdoc />
    public override void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime)
    {
        allureRuntime.LifecycleApi.StartTestScope(new() { Uuid = Ids.NewUuid()  });
    }
}
