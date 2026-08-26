using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureGlobalAttachment(JsonElement Json) :
    IAllureAttachment<AllureGlobalAttachment>,
    IAllureTimestampProperty<AllureGlobalAttachment>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureGlobalAttachment Constructor(JsonElement json) => new(json);
}
