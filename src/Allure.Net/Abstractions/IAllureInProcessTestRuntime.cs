namespace Allure.Abstractions;

public class AllureBackendTestApi(
    IAllureInProcessTestApi sync,
    IAllureInProcessTestApiAsync @async
)
{
    public IAllureInProcessTestApi Sync => sync;

    public IAllureInProcessTestApiAsync Async => @async;
}
