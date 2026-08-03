namespace Allure.Abstractions;

/// <summary>
/// Converts CLR values to text for Allure parameters and interpolated names.
/// </summary>
public interface IAllureParameterSerializer
{
    /// <summary>
    /// Serializes a CLR value.
    /// </summary>
    string Serialize(object? value);
}
