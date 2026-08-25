using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureGlobals(JsonElement Json) :
    IAllureModelObject<AllureGlobals>,
    IAllureGlobalAttachmentsProperty<AllureGlobals>,
    IAllureGlobalErrorsProperty<AllureGlobals>
{
    public static string? Validate(JsonElement json) => json switch
    {
        { ValueKind: JsonValueKind.Null } =>
            "the globals element was null",

        { ValueKind: not JsonValueKind.Object } =>
            "the globals element was not a JSON object",

        _ => null,
    };

    public static AllureGlobals Constructor(JsonElement json) => new(json);
}
