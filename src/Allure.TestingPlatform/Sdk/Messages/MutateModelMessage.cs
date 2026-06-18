namespace Allure.TestingPlatform.Sdk.Messages;

public abstract class MutateModelMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid,
    IAllureContextUid contextUid
) :
    DataWithAllureProperties(displayName, description, correlationUid),
    IAllureLifecycleMessage
{
    public IAllureContextUid ContextUid { get; } = contextUid;

    public abstract void Mutate(IAllureInfrastructure allure);
}