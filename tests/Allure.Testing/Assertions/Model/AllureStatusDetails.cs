using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureStatusDetails(JsonElement Json) :
    IAllureModelObject<AllureStatusDetails>,
    IAllureFlakyProperty<AllureStatusDetails>,
    IAllureKnownProperty<AllureStatusDetails>,
    IAllureMessageProperty<AllureStatusDetails>,
    IAllureMutedProperty<AllureStatusDetails>,
    IAllureTraceProperty<AllureStatusDetails>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureStatusDetails Constructor(JsonElement json) => new(json);
}
