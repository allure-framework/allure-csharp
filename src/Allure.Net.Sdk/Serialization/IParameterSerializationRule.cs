using System.Diagnostics.CodeAnalysis;

namespace Allure.Sdk.Serialization;

/// <summary>
/// Attempts to serialize supported parameter values to text.
/// </summary>
public interface IParameterSerializationRule
{
    /// <summary>
    /// Attempts to serialize a value.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <param name="text">The serialized text when the rule accepts the value.</param>
    /// <returns>
    /// <see langword="true"/> if the rule accepted the value; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    bool TrySerialize(object value, [NotNullWhen(true)] out string? text);
}
