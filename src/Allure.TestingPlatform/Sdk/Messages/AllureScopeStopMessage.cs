using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureScopeStopMessage(
    CorrelationUid correlationUid,
    ScopeContextUid scopeUid
) :
    AllureModelRemoveMessage(
        "Allure scope stop",
        "This message reports that an Allure test scope has stopped.",
        correlationUid,
        scopeUid
    )
{
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    public override void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime)
    {
        allureRuntime.Lifecycle
            .StopTestContainer()
            .WriteTestContainer();
    }
}
