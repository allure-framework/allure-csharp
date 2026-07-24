using System.Diagnostics.CodeAnalysis;

namespace Allure.Sdk.Serialization;

public interface IParameterSerializationRule
{
    bool TrySerialize(object value, [NotNullWhen(true)] out string? text);
}
