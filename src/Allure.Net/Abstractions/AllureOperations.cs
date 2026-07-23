namespace Allure.Abstractions;

/// <summary>
/// Groups the synchronous and asynchronous operations of an in-process runtime endpoint.
/// </summary>
public class AllureOperations(
    IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext> sync,
    IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> @async
)
{
    /// <summary>
    /// Gets the synchronous operations.
    /// </summary>
    public IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext> Sync => sync;

    /// <summary>
    /// Gets the asynchronous operations.
    /// </summary>
    public IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> Async => @async;
}
