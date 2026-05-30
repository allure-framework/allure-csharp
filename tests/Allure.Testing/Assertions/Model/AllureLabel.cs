using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureLabel(JsonElement Json) :
    IAllureModelObject<AllureLabel>,
    IAllureNameProperty<AllureLabel>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureLabel Constructor(JsonElement json) => new(json);
}
