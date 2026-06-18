namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureScopeStopMessage(
    CorrelationUid correlationUid,
    ScopeContextUid scopeUid
) :
    RemoveContextMessage(
        "Allure scope stop",
        "This message reports that an Allure test scope has stopped.",
        correlationUid,
        scopeUid
    )
{
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    public override void Mutate(IAllureInfrastructure allure)
    {
        allure.Lifecycle
            .StopTestContainer()
            .WriteTestContainer();
    }
}
