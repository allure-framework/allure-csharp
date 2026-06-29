using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

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

    public abstract void Mutate(LiveAllureTestingPlatformRuntime allureState);
}