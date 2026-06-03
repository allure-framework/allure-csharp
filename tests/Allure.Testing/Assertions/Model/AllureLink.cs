using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureLink(JsonElement Json) :
    IAllureModelObject<AllureLink>,
    IAllureNameProperty<AllureLink>,
    IAllureLinkTypeProperty<AllureLink>,
    IAllureLinkUrlProperty<AllureLink>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureLink Constructor(JsonElement json) => new(json);
}
