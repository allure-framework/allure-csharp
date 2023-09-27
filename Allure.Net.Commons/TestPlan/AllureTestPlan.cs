using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#nullable enable

namespace Allure.Net.Commons.TestPlan;

/// <summary>
/// Used by integrations to enable selective run of tests.
/// </summary>
public class AllureTestPlan
{
    /// <summary>
    /// Testplan entries that denote tests included in the run.
    /// </summary>
    public List<AllureTestPlanItem> Tests
    {
        get => this.tests;
        set
        {
            this.tests = value ?? new();
            this.RecreateFilters();
        }
    }

    /// <summary>
    /// Used by integrations to check if a test is selected by the testplan.
    /// </summary>
    /// <param name="fullName">A fullName of the test.</param>
    /// <param name="allureId">
    /// An identifier of the test case (if any).
    /// Use <see cref="GetAllureId"/> to get it from the test result.
    /// </param>
    /// <returns>true if the test should be executed. false otherwise.</returns>
    public bool IsMatch(string fullName, string? allureId) =>
        this.IsDefaultTestplanMatch()
            || this.IsFullNameMatch(fullName)
            || this.IsAllureIdMatch(allureId);

    /// <summary>
    /// A shorthand for <see cref="IsMatch(string, string?)"/> with the
    /// fullName and the allure id taken from the provided test result.
    /// </summary>
    public bool IsMatch(TestResult testResult) =>
        this.IsMatch(
            testResult.fullName,
            GetAllureId(testResult.labels)
        );

    /// <summary>
    /// Creates the testplan from a JSON string.
    /// </summary>
    public static AllureTestPlan FromJson(string jsonContent) =>
        JsonConvert.DeserializeObject<AllureTestPlan>(
            jsonContent,
            new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
            }
        ) ?? DEFAULT_TESTPLAN;

    /// <summary>
    /// Creates the testplan from the file pointed by the environment variable.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public static AllureTestPlan FromEnvironment()
    {
        var testPlanPath = ResolveTestPlanPath();
        return GetTestPlanByPath(testPlanPath);
    }

    /// <summary>
    /// Finds an Allure id in a sequence of labels. If no id exists, returns
    /// null.
    /// </summary>
    public static string? GetAllureId(IEnumerable<Label> labels) =>
        FindLabel(labels, AllureConstants.NEW_ALLURE_ID_LABEL_NAME)
            ?? FindLabel(labels, AllureConstants.OLD_ALLURE_ID_LABEL_NAME);

    /// <summary>
    /// A testplan that doesn't filter any test.
    /// </summary>
    public static readonly AllureTestPlan DEFAULT_TESTPLAN = new();

    static string? ResolveTestPlanPath() =>
        new[]
        {
            AllureConstants.NEW_ALLURE_TESTPLAN_ENV_NAME,
            AllureConstants.OLD_ALLURE_TESTPLAN_ENV_NAME
        }.Select(Environment.GetEnvironmentVariable).Where(
            ev => !string.IsNullOrWhiteSpace(ev)
        ).FirstOrDefault();

    static AllureTestPlan GetTestPlanByPath(string? testPlanPath) =>
        testPlanPath is null || !File.Exists(testPlanPath)
            ? DEFAULT_TESTPLAN
            : ReadTestPlanFromFile(testPlanPath);

    static AllureTestPlan ReadTestPlanFromFile(string testPlanPath) =>
        FromJson(
            File.ReadAllText(testPlanPath, Encoding.UTF8)
        );

    static string? FindLabel(IEnumerable<Label> labels, string labelName) =>
        labels.FirstOrDefault(
            l => labelName.Equals(l.name, StringComparison.OrdinalIgnoreCase)
        )?.value;

    List<AllureTestPlanItem> tests = new();
    HashSet<string> allIds = new();
    HashSet<string> allSelectors = new();

    void RecreateFilters()
    {
        this.allIds = new HashSet<string>(
            from entry in this.tests
            where entry.Id is not null
            select entry.Id,
            StringComparer.Ordinal
        );
        this.allSelectors = new HashSet<string>(
            from entry in this.tests
            where entry.Selector is not null
            select entry.Selector,
            StringComparer.Ordinal
        );
    }

    bool IsDefaultTestplanMatch() => !this.tests.Any();

    bool IsAllureIdMatch(string? allureId) =>
        allureId is not null && this.allIds.Contains(allureId);

    bool IsFullNameMatch(string fullName) =>
        this.allSelectors.Contains(fullName);
}
