namespace Allure.Abstractions;

/// <summary>Groups the synchronous and asynchronous operations of an API endpoint.</summary>
public class AllureApiOperations(
    IAllureOperations<IAllureStepContext, IAllureFixtureContext> sync,
    IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> @async
)
{
    /// <summary>Gets the synchronous operations.</summary>
    public IAllureOperations<IAllureStepContext, IAllureFixtureContext> Sync => sync;

    /// <summary>Gets the asynchronous operations.</summary>
    public IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> Async => @async;
}
