namespace Allure.Abstractions;

public class AllureApiOperations(
    IAllureOperations<IAllureStepContext, IAllureFixtureContext> sync,
    IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> @async
)
{
    public IAllureOperations<IAllureStepContext, IAllureFixtureContext> Sync => sync;

    public IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext> Async => @async;
}
