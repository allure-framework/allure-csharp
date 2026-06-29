using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

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

    public abstract void Mutate(ReadyAllureTestingPlatformRuntime allureState);
}