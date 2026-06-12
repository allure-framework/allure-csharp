using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public sealed class AllureScopeStopMessage(SessionUid sessionUid, string scopeUid) :
    RemoveContextMessage(
        "Allure scope stop",
        "This message reports that an Allure test scope has stopped.",
        sessionUid,
        scopeUid
    )
{
    public override void Mutate(IAllureInfrastructure allure)
    {
        allure.Lifecycle
            .StopTestContainer()
            .WriteTestContainer();
    }
}
