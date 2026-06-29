using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;

namespace Allure.TestingPlatform.Sdk.Messages;

public interface IAllureLifecycleMessage
{
    IAllureContextUid ContextUid { get; }

    void Mutate(ReadyAllureTestingPlatformRuntime allureState);
}
