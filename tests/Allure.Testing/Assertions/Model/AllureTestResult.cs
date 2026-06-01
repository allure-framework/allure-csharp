using System;
using System.Text.Json;
using Allure.Testing.Assertions.Model.Properties;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureTestResult(JsonElement Json) :
    IAllureExecutableItem<AllureTestResult>,
    IAllureHistoryIdProperty<AllureTestResult>,
    IAllureLabelsProperty<AllureTestResult>,
    IAllureLinksProperty<AllureTestResult>,
    IAllureTestCaseIdProperty<AllureTestResult>,
    IAllureTitlePathProperty<AllureTestResult>,
    IAllureUuidProperty<AllureTestResult>
{
    public static string? Validate(JsonElement json) => json switch
    {
        { ValueKind: JsonValueKind.Null } =>
            "the test result was null",

        { ValueKind: not JsonValueKind.Object } =>
            "the test result was not a JSON object",

        _ => CheckProperties(json),
    };

    static string? CheckProperties(JsonElement testResult)
    {
        if (CheckUuid(testResult) is {} badUuid)
        {
            return badUuid;
        }

        if (CheckName(testResult) is {} badName)
        {
            return badName;
        }

        if (CheckStatus(testResult) is {} badStatus)
        {
            return badStatus;
        }

        if (CheckTestCaseId(testResult) is {} badTestCaseId)
        {
            return badTestCaseId;
        }

        if (CheckHistoryId(testResult) is {} badHistoryId)
        {
            return badHistoryId;
        }

        return null;
    }

    public static string? CheckUuid(JsonElement testResult) =>
        IAllureUuidProperty<AllureTestResult>.GetValue(testResult, "uuid") is { IsPassed: false, Message: var error}
            ? error
            : null;

    public static string? CheckName(JsonElement testResult) =>
        IAllureNameProperty<AllureTestResult>.GetValue(testResult, "name") is { IsPassed: false, Message: var error}
            ? error
            : null;

    static string? CheckStatus(JsonElement testResult) =>
        IAllureStatusProperty<AllureTestResult>.GetValue(testResult, "status") is { IsPassed: false, Message: var error}
            ? error
            : null;

    public static string? CheckTestCaseId(JsonElement testResult) =>
        IAllureTestCaseIdProperty<AllureTestResult>.GetValue(testResult, "testCaseId") is { IsPassed: false, Message: var error}
            ? error
            : null;

    public static string? CheckHistoryId(JsonElement testResult) =>
        IAllureHistoryIdProperty<AllureTestResult>.GetValue(testResult, "historyId") is { IsPassed: false, Message: var error}
            ? error
            : null;

    public static AllureTestResult Constructor(JsonElement json) => new(json);
}
