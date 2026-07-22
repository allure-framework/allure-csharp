namespace Allure.Abstractions;

/// <summary>
/// Represents an in-process Allure runtime registered as a backend.
/// </summary>
public interface IAllureRuntime
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
    /// Gets the runtime's in-process operations.
    /// </summary>
    AllureRuntimeOperations Operations { get; }

    /// <summary>
    /// Gets the serializer used for CLR parameter values.
    /// </summary>
    IAllureParameterSerializer ParameterSerializer { get; }
}
