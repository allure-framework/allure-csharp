namespace Allure.Abstractions;

/// <summary>
/// Represents a resolved destination for Allure API calls.
/// </summary>
public interface IAllureApiEndpoint
{
    /// <summary>
    /// Gets the operations exposed by this endpoint.
    /// </summary>
    AllureApiOperations Operations { get; }

    /// <summary>
    /// Gets the serializer used for CLR parameter values.
    /// </summary>
    IAllureParameterSerializer ParameterSerializer { get; }
}
