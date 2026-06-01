using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureContainer(JsonElement Json) :
    IAllureModelObject<AllureContainer>,
    IAllureAftersProperty<AllureContainer>,
    IAllureBeforesProperty<AllureContainer>,
    IAllureChildrenProperty<AllureContainer>,
    IAllureNameProperty<AllureContainer>,
    IAllureUuidProperty<AllureContainer>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureContainer Constructor(JsonElement json) => new(json);
}
