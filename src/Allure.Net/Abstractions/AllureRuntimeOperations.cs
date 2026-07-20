namespace Allure.Abstractions;

public class AllureRuntimeOperations(
    IAllureInProcessOperations sync,
    IAllureAsyncInProcessOperations @async
)
{
    public IAllureInProcessOperations Sync => sync;

    public IAllureAsyncInProcessOperations Async => @async;
}
