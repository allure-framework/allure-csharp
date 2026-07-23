namespace Allure.Abstractions;

/// <summary>
/// Groups the synchronous and asynchronous operations of an in-process runtime endpoint.
/// </summary>
public class AllureInProcessOperations(
    IAllureInProcessSyncOperations sync,
    IAllureInProcessAsyncOperations @async
)
{
    /// <summary>
    /// Gets the synchronous operations.
    /// </summary>
    public IAllureInProcessSyncOperations Sync => sync;

    /// <summary>
    /// Gets the asynchronous operations.
    /// </summary>
    public IAllureInProcessAsyncOperations Async => @async;
}
