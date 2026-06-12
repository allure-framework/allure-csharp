using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public abstract class RemoveContextMessage(
    string displayName,
    string description,
    SessionUid sessionUid,
    string contextUid
) :
    DataWithAllureProperties(displayName, description, sessionUid),
    IAllureLifecycleMessage
{
    public string ContextUid { get; } = contextUid;

    public abstract void Mutate(IAllureInfrastructure allure);
}