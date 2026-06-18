namespace Allure.TestingPlatform.Sdk.Messages;

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