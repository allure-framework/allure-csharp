using Allure.Abstractions;

namespace Allure.Net.Sdk.Tests.Infrastructure;

sealed class RecordingParameterSerializer(
    Func<object?, string> serialize
) : IAllureParameterSerializer
{
    readonly List<object?> values = [];

    public IReadOnlyList<object?> Values => this.values;

    public string Serialize(object? value)
    {
        this.values.Add(value);
        return serialize(value);
    }
}
