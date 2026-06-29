using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

public interface IAllureModelOperationMessage
{
    IAllureContextUid ContextUid { get; }

    void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime);
}
