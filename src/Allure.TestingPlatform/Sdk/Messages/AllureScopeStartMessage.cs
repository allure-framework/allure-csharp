using Allure.Net.Commons.Functions;

namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureScopeStartMessage(
    CorrelationUid correlationUid,
    ScopeContextUid scopeUid,
    ScopeContextUid? parentScopeUid = null
) :
    CreateContextMessage(
        "Allure scope start",
        "This message reports that an Allure test scope has started.",
        correlationUid,
        scopeUid,
        parentScopeUid
    )
{
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    public ScopeContextUid? ParentScopeUid { get; } = parentScopeUid;

    public override void Mutate(IAllureRuntime allure)
    {
        allure.Lifecycle.StartTestContainer(new() { uuid = IdFunctions.CreateUUID() });
    }
}
