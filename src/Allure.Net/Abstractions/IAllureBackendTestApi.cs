namespace Allure.Abstractions;

public interface IAllureBackendTestApi
{
    IAllureInProcessTestApi SyncApi { get; }

    IAllureInProcessTestApiAsync AsyncApi { get; }
}
