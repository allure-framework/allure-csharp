using System.Diagnostics.CodeAnalysis;

namespace Allure.Sdk.Serialization;

/// <summary>
/// Provides a base for serialization rules that accept values of a specific type.
/// </summary>
/// <typeparam name="T">The supported value type.</typeparam>
public abstract class TypedParameterSerializationRule<T> : IParameterSerializationRule
{
    /// <inheritdoc/>
    public bool TrySerialize(object value, [NotNullWhen(true)] out string? text)
    {
        if (value is T supportedValue)
        {
            text = this.Serialize(supportedValue);
            return true;
        }

        text = null;
        return false;
    }

    /// <summary>
    /// Serializes a value known to be of the supported type.
    /// </summary>
    protected abstract string Serialize(T value);
}
