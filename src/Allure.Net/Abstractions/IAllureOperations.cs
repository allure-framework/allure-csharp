namespace Allure.Abstractions;

/// <summary>
/// Groups the synchronous and asynchronous operations of an in-process runtime endpoint.
/// </summary>
public interface IAllureOperations
{
    /// <summary>
    /// Gets the synchronous operations.
    /// </summary>
    IAllureSyncOperations<IAllureStepContext, IAllureFixtureContext> Sync { get; }

    /// <summary>
    /// Gets the asynchronous operations.
    /// </summary>
    IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> Async { get; }
}
