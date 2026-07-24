using System.Diagnostics.CodeAnalysis;

namespace Allure.Sdk.Serialization;

public abstract class TypedParameterSerializationRule<T> : IParameterSerializationRule
{
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

    protected abstract string Serialize(T value);
}
