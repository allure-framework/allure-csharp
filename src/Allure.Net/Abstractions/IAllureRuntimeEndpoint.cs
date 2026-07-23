namespace Allure.Abstractions;

/// <summary>
/// Represents an endpoint to an Allure runtime.
/// </summary>
public interface IAllureRuntimeEndpoint
{
    /// <summary>
    /// Gets the runtime name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets whether the runtime can currently perform useful work.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Gets the runtime-specific implementation of the API operations.
    /// </summary>
    IAllureOperations Operations { get; }

    /// <summary>
    /// Gets the serializer used for CLR parameter values.
    /// </summary>
    IAllureParameterSerializer ParameterSerializer { get; }
}
