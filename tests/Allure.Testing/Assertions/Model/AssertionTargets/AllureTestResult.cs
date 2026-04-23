using System.Text.Json;
using Allure.Testing.Internal;
using TUnit.Assertions.Conditions.Json;

namespace Allure.Testing.Assertions.Model.AssertionTargets;

public readonly record struct AllureTestResult(JsonElement Json): IAllureExecutableItem
{
    public string? Validate() => Validate(this.Json);

    public static string? Validate(JsonElement testResult)
    {
        return testResult switch
        {
            _ when testResult.IsNull() =>
                "the test result was null",

            _ when !testResult.IsObject() =>
                "the test result was not a JSON object",

            _ when !HasValidUuid(testResult) =>
                "the test result didn't have a valid UUID",

            _ when !HasValidStatus(testResult) =>
                "the test result didn't have a valid status",

            _ when !HasTestCaseId(testResult) =>
                "the test result didn't have a valid testCaseId",

            _ when !HasHistoryId(testResult) =>
                "the test result didn't have a valid historyId",

            _ => null,
        };
    }

    public static bool HasValidUuid(JsonElement testResult) =>
        testResult.TryGetProperty("uuid", out var propertyValue) && propertyValue.TryGetGuid(out var _);

    public static bool HasTestCaseId(JsonElement testResult) =>
        JsonFunctions.GetStringProperty(testResult, "testCaseId") is not null;

    public static bool HasHistoryId(JsonElement testResult) =>
        JsonFunctions.GetStringProperty(testResult, "historyId") is not null;

    public static bool HasValidStatus(JsonElement testResult) =>
        JsonFunctions.GetStringProperty(testResult, "status") switch
        {
            "passed" or "failed" or "broken" or "skipped" or "unknown" => true,
            _ => false
        };
}
