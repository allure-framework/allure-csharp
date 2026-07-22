namespace Allure.Abstractions;

/// <summary>
/// Defines asynchronous operations that require an in-process Allure runtime.
/// </summary>
public interface IAllureAsyncInProcessOperations :
    IAllureAsyncOperations<IAllureAsyncInProcessStepContext, IAllureAsyncInProcessFixtureContext>
{
}
