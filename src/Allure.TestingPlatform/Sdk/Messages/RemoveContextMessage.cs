using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

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

    public abstract void Mutate(ReadyAllureTestingPlatform allureState);
}