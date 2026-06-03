using System.Text.Json;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureFixtureResult(JsonElement Json)
    : IAllureExecutableItem<AllureFixtureResult>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureFixtureResult Constructor(JsonElement json) => new(json);
}
