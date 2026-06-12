namespace Allure.TestingPlatform.Messages;

public interface IAllureLifecycleMessage
{
    string ContextUid { get; }

    void Mutate(IAllureInfrastructure allure);
}
