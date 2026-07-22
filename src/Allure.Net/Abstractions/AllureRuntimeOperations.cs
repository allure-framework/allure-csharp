namespace Allure.Abstractions;

/// <summary>
/// Groups the synchronous and asynchronous operations of an in-process runtime.
/// </summary>
public class AllureRuntimeOperations(
    IAllureInProcessOperations sync,
    IAllureAsyncInProcessOperations @async
)
{
    /// <summary>
    /// Gets the synchronous in-process operations.
    /// </summary>
    public IAllureInProcessOperations Sync => sync;

    /// <summary>
    /// Gets the asynchronous in-process operations.
    /// </summary>
    public IAllureAsyncInProcessOperations Async => @async;
}
