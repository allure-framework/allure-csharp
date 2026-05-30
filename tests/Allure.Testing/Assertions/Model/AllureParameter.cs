using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureParameter(JsonElement Json) :
    IAllureModelObject<AllureParameter>,
    IAllureNameProperty<AllureParameter>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureParameter Constructor(JsonElement json) => new(json);
}
