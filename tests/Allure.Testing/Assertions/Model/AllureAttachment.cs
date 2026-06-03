using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureAttachment(JsonElement Json) :
    IAllureModelObject<AllureAttachment>,
    IAllureNameProperty<AllureAttachment>,
    IAllureAttachmentSourceProperty<AllureAttachment>,
    IAllureLinkTypeProperty<AllureAttachment>
{
    public static string? Validate(JsonElement json) => default;

    public static AllureAttachment Constructor(JsonElement json) => new(json);
}
