using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureScopeStartMessage(
    CorrelationUid correlationUid,
    ScopeContextUid scopeUid,
    ScopeContextUid? parentScopeUid = null
) :
    AllureModelCreateMessage(
        "Allure scope start",
        "This message reports that an Allure test scope has started.",
        correlationUid,
        scopeUid,
        parentScopeUid
    )
{
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    public ScopeContextUid? ParentScopeUid { get; } = parentScopeUid;

    public override void ApplyTo(LiveAllureTestingPlatformRuntime allureState)
    {
        allureState.Lifecycle.StartTestContainer(new() { uuid = IdFunctions.CreateUUID() });
    }
}
