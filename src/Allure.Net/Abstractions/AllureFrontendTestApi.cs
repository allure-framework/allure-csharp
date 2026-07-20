namespace Allure.Abstractions;

public class AllureFrontendTestApi(
    IAllureTestApi<IAllureStepContext, IAllureFixtureContext> sync,
    IAllureTestApiAsync<IAllureStepContextAsync, IAllureFixtureContextAsync> @async
)
{
    public IAllureTestApi<IAllureStepContext, IAllureFixtureContext> Sync => sync;

    public IAllureTestApiAsync<IAllureStepContextAsync, IAllureFixtureContextAsync> Async => @async;
}
