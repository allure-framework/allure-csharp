namespace Allure.TestingPlatform.Sdk.Messages;

public interface IAllureLifecycleMessage
{
    IAllureContextUid ContextUid { get; }

    void Mutate(IAllureInfrastructure allure);
}
