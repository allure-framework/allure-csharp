using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public abstract class CreateContextMessage(
    string displayName,
    string description,
    SessionUid sessionUid,
    string contextUid,
    string? parentContextUid
) :
    DataWithAllureProperties(displayName, description, sessionUid),
    IAllureLifecycleMessage
{
    public string ContextUid { get; } = contextUid;

    public string? ParentContextUid { get; } = parentContextUid;

    public abstract void Mutate(IAllureInfrastructure allure);
}