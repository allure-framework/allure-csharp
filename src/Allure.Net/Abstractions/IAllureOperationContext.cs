namespace Allure.Abstractions;

/// <summary>
/// Provides services available while an Allure operation is running.
/// </summary>
public interface IAllureOperationContext
{
    /// <summary>
    /// Gets the serializer used for CLR parameter values.
    /// </summary>
    IAllureParameterSerializer ParameterSerializer { get; }
}
