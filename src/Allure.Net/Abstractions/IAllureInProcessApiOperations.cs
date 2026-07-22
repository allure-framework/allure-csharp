namespace Allure.Abstractions;

/// <summary>
/// Groups the synchronous and asynchronous operations of an in-process runtime endpoint.
/// </summary>
public interface IAllureInProcessApiOperations
{
    /// <summary>
    /// Gets the synchronous operations.
    /// </summary>
    IAllureInProcessOperations Sync { get; }

    /// <summary>
    /// Gets the asynchronous operations.
    /// </summary>
    IAllureAsyncInProcessOperations Async { get; }
}
