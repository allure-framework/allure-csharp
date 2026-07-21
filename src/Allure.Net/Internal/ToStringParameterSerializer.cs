using Allure.Abstractions;

namespace Allure.Internal;

class ToStringParameterSerializer : IAllureParameterSerializer
{
    public string Serialize(object? value) =>
        value?.ToString() ?? "null";

    public static ToStringParameterSerializer Instance { get; } = new();
}