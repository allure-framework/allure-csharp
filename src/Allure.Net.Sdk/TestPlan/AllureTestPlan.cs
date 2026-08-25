using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Allure.Model;

namespace Allure.Sdk.TestPlan;

/// <summary>
/// Represents a test plan that selects a subset of tests to run.
/// </summary>
public record class AllureTestPlan
{
    static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
    };

    readonly ImmutableArray<AllureTestPlanEntry> tests = [];

    readonly ImmutableHashSet<string> ids = [];

    readonly ImmutableHashSet<string> selectors = [];

    /// <summary>
    /// Gets the test plan entries.
    /// </summary>
    public ImmutableArray<AllureTestPlanEntry> Tests
    {
        get => this.tests;
        init
        {
            this.tests = value;
            this.ids = [
                .. from entry in this.Tests
                where entry.Id is not null
                select entry.Id
            ];
            this.selectors = [
                .. from entry in this.Tests
                where entry.Selector is not null
                select entry.Selector,
            ];
        }
    }

    /// <summary>
    /// Checks if a test is selected by the test plan.
    /// </summary>
    /// <param name="fullName">A fullName of the test.</param>
    /// <param name="allureId">
    /// An identifier of the test case (if any).
    /// Use <see cref="GetAllureId"/> to get it from the test result.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the test case should be executed.
    /// <see langword="false"/> otherwise.
    /// </returns>
    public bool IsSelected(string? fullName, string? allureId) =>
        this.IsFullNameMatch(fullName)
            || this.IsAllureIdMatch(allureId);

    /// <summary>
    /// A shorthand for <see cref="IsSelected(string, string?)"/> with the
    /// fullName and the allure id taken from the provided test result.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the test case should be executed.
    /// <see langword="false"/> otherwise.
    /// </returns>
    public bool IsSelected(TestResult testResult) =>
        this.IsSelected(
            testResult.FullName,
            GetAllureId(testResult.Labels)
        );

    /// <summary>
    /// Creates the test plan from a JSON string.
    /// </summary>
    public static AllureTestPlan FromJson(string jsonContent) =>
        JsonSerializer.Deserialize<AllureTestPlan>(
            jsonContent,
            jsonOptions
        )
            ?? throw new InvalidOperationException(
                "Could not parse the test plan."
            );

    /// <summary>
    /// Loads the test plan from the file pointed by the
    /// <c>ALLURE_TESTPLAN_PATH</c> environment variable.
    /// </summary>
    /// <returns>
    /// The loaded test plan or <see langword="null"/> if
    /// the environment variable is not set or the file does not exist.
    /// </returns>
    public static AllureTestPlan? FromEnvironment()
    {
        var testPlanPath = ResolveTestPlanPath();
        return GetTestPlanByPath(testPlanPath);
    }

    /// <summary>
    /// Finds an Allure id in a sequence of labels. If no id exists, returns
    /// null.
    /// </summary>
    public static string? GetAllureId(IEnumerable<Label> labels) =>
        FindLabel(labels, "ALLURE_ID")
            ?? FindLabel(labels, "AS_ID");

    /// <summary>
    /// Returns the path of the current test plan file, if any.
    /// </summary>
    public static string? ResolveTestPlanPath() =>
        Environment.GetEnvironmentVariable("ALLURE_TESTPLAN_PATH") is { Length: > 0 } path
            ? path
            : Environment.GetEnvironmentVariable("AS_TESTPLAN_PATH") is { Length: > 0 } legacyPath
                ? legacyPath
                : null;

    /// <summary>
    /// A short message that can be used to report an ignored test to the test
    /// framework.
    /// </summary>
    public const string SkipReason =
        "The test case is not in the test plan.";

    bool IsAllureIdMatch(string? allureId) =>
        allureId is not null && this.ids.Contains(allureId);

    bool IsFullNameMatch(string? fullName) =>
        fullName is { Length: >0 }
            && this.selectors.Contains(fullName);

    static AllureTestPlan? GetTestPlanByPath(string? testPlanPath) =>
        testPlanPath is not null && File.Exists(testPlanPath)
            ? ReadTestPlanFromFile(testPlanPath)
            : null;

    static AllureTestPlan ReadTestPlanFromFile(string testPlanPath) =>
        FromJson(
            File.ReadAllText(testPlanPath, Encoding.UTF8)
        );

    static string? FindLabel(IEnumerable<Label> labels, string labelName) =>
        labels.FirstOrDefault(
            l => labelName.Equals(l.Name, StringComparison.OrdinalIgnoreCase)
        )?.Value;
}
