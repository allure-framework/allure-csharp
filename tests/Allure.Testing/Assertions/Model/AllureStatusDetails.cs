using System.Text.Json;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureStatusDetails(JsonElement Json) :
    IAllureStatusDetails<AllureStatusDetails>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureStatusDetails Constructor(JsonElement json) => new(json);
}
