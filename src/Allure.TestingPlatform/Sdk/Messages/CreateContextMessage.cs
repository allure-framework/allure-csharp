using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

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

    public abstract void Mutate(ReadyAllureTestingPlatformRuntime allureState);
}