using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class TestParameterSerializer(string prefix = "serialized") : IAllureParameterSerializer
{
    public string Serialize(object? value) => $"{prefix}:{value}";
}
