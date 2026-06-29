using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

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

    public abstract void Mutate(LiveAllureTestingPlatformRuntime allureState);
}