using System.Diagnostics.CodeAnalysis;

namespace Allure.Sdk.Serialization;

/// <summary>
/// Serializes parameter values by calling <see cref="object.ToString"/>.
/// </summary>
public class ToStringParameterSerializationRule : IParameterSerializationRule
{
    /// <inheritdoc/>
    public bool TrySerialize(object value, [NotNullWhen(true)] out string? text)
    {
        text = value.ToString();
        return true;
    }

    /// <summary>
    /// Gets the shared rule instance.
    /// </summary>
    public static ToStringParameterSerializationRule Instance { get; } = new();
}
