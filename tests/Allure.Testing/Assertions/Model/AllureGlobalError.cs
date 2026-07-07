using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureGlobalError(JsonElement Json) :
    IAllureStatusDetails<AllureGlobalError>,
    IAllureTimestampProperty<AllureGlobalError>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureGlobalError Constructor(JsonElement json) => new(json);
}
