using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Messages;

public abstract class RemoveContextMessage(
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