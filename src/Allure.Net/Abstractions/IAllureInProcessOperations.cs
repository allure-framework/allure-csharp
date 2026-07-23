namespace Allure.Abstractions;

/// <summary>
/// Groups the synchronous and asynchronous operations of an in-process runtime endpoint.
/// </summary>
public interface IAllureInProcessOperations
{
    /// <summary>
    /// Gets the synchronous operations.
    /// </summary>
    IAllureInProcessSyncOperations Sync { get; }

    /// <summary>
    /// Gets the asynchronous operations.
    /// </summary>
    IAllureInProcessAsyncOperations Async { get; }
}
