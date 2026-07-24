using System.Diagnostics.CodeAnalysis;

namespace Allure.Sdk.Serialization;

public class ToStringParameterSerializationRule : IParameterSerializationRule
{
    public bool TrySerialize(object value, [NotNullWhen(true)] out string? text)
    {
        text = value.ToString();
        return true;
    }

    public static ToStringParameterSerializationRule Instance { get; } = new();
}
