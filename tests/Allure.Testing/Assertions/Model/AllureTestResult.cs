using System;
using System.Text.Json;
using Allure.Testing.Internal;

namespace Allure.Testing.Assertions.Model;

public readonly record struct AllureTestResult(JsonElement Json)
    : IAllureExecutableItem<AllureTestResult>
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
        JsonFunctions.GetStringProperty(testResult, "uuid") switch
        {
            { IsPassed: true, Value: var value } when Guid.TryParse(value, out _) =>
                null,

            { IsPassed: true, Value: var value } =>
                $"{value} is not a valid UUID",

            { Message: var error } => error,
        };

    public static string? CheckName(JsonElement testResult) =>
        JsonFunctions.GetStringProperty(testResult, "name") is { IsPassed: false, Message: var error }
            ? error
            : null;

    public static string? CheckStatus(JsonElement testResult) =>
        JsonFunctions.GetStringProperty(testResult, "status") switch
        {
            { IsPassed: true, Value: "passed" or "failed" or "broken" or "skipped" or "unknown" } =>
                null,

            { IsPassed: true, Value: var value } => $"got an unexpected status {value}",

            { Message: var error } => error,
        };

    public static string? CheckTestCaseId(JsonElement testResult) =>
        JsonFunctions.GetStringProperty(testResult, "testCaseId") is { IsPassed: false, Message: var error }
            ? error
            : null;

    public static string? CheckHistoryId(JsonElement testResult) =>
        JsonFunctions.GetStringProperty(testResult, "historyId") is { IsPassed: false, Message: var error }
            ? error
            : null;

    public static AllureTestResult Constructor(JsonElement json) => new(json);
}
