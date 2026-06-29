using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

public abstract class AllureModelCreateMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid,
    IAllureContextUid contextUid,
    IAllureContextUid? parentContextUid
) :
    AllureModelMessage(displayName, description, correlationUid),
    IAllureModelOperationMessage
{
    public IAllureContextUid ContextUid { get; } = contextUid;

    public IAllureContextUid? ParentContextUid { get; } = parentContextUid;

    public abstract void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime);
}