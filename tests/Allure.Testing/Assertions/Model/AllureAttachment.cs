using System.Text.Json;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureAttachment(JsonElement Json) :
    IAllureAttachment<AllureAttachment>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureAttachment Constructor(JsonElement json) => new(json);
}
