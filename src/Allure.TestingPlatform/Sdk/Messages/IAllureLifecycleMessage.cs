using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;

namespace Allure.TestingPlatform.Sdk.Messages;

public interface IAllureLifecycleMessage
{
    IAllureContextUid ContextUid { get; }

    void Mutate(ReadyAllureTestingPlatform allureState);
}
