using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Messages;

public interface IAllureLifecycleMessage
{
    IAllureContextUid ContextUid { get; }

    void Mutate(IAllureInfrastructure allure);
}
