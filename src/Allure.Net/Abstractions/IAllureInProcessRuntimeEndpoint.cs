namespace Allure.Abstractions;

/// <summary>
/// Represents an endpoint to an in-process Allure runtime.
/// </summary>
public interface IAllureInProcessRuntimeEndpoint : IAllureRuntimeEndpoint
{
    /// <summary>
    /// Gets the runtime-specific implementation of the in-process API operations.
    /// </summary>
    IAllureInProcessOperations InProcessOperations { get; }
}
