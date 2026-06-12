using Allure.Net.Commons.Functions;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public sealed class AllureScopeStartMessage(
    SessionUid sessionUid,
    string scopeUid,
    string? parentScopeUid = null
) :
    CreateContextMessage(
        "Allure scope start",
        "This message reports that an Allure test scope has started.",
        sessionUid,
        scopeUid,
        parentScopeUid
    )
{
    public override void Mutate(IAllureInfrastructure allure)
    {
        allure.Lifecycle.StartTestContainer(new() { uuid = IdFunctions.CreateUUID() });
    }
}
