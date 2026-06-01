using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;
using Allure.Testing.Internal;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureStepResult(JsonElement Json)
    : IAllureExecutableItem<AllureStepResult>
{
    public static AllureStepResult Constructor(JsonElement json) => new(json);

    public static string? Validate(JsonElement json) => json switch
    {
        { ValueKind: JsonValueKind.Null } =>
            "was null",

        { ValueKind: not JsonValueKind.Object } =>
            "was not a JSON object",

        _ => CheckProperties(json),
    };

    static string? CheckProperties(JsonElement stepResult)
    {
        if (CheckName(stepResult) is {} badName)
        {
            return badName;
        }

        if (CheckStatus(stepResult) is {} badStatus)
        {
            return badStatus;
        }

        return null;
    }

    static string? CheckName(JsonElement stepResult) =>
        JsonFunctions.GetStringProperty(stepResult, "name") is { IsPassed: false, Message: var error }
            ? error
            : null;

    static string? CheckStatus(JsonElement stepResult) =>
        IAllureStatusProperty<AllureStepResult>.GetValue(stepResult, "status") is { IsPassed: false, Message: var error}
            ? error
            : null;
}
