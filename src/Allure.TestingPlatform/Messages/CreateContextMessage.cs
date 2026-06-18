using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Messages;

public abstract class CreateContextMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid,
    IAllureContextUid contextUid,
    IAllureContextUid? parentContextUid
) :
    DataWithAllureProperties(displayName, description, correlationUid),
    IAllureLifecycleMessage
{
    public IAllureContextUid ContextUid { get; } = contextUid;

    public IAllureContextUid? ParentContextUid { get; } = parentContextUid;

    public abstract void Mutate(IAllureInfrastructure allure);
}