using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.TestPlan;
using Xunit;

namespace Allure.Xunit.Functions;

public static class TestPlanFunctions
{
    static readonly Lazy<AllureTestPlan> testPlanLazy = new(AllureTestPlan.FromEnvironment);

    public static AllureTestPlan TestPlan => testPlanLazy.Value;

    /// <summary>
    /// Returns an array consisting of the original CLI arguments plus filter arguments that,
    /// when passed to xUnit.net v3, enforces the global test plan for the entry point assembly.
    /// </summary>
    /// <param name="args">An array of command line arguments to the application.</param>
    /// <param name="allureIdRegistry">
    /// A mapping from Allure ID to test method names.
    /// </param>
    /// <returns>
    /// A sequence of xUnit.net arguments in form
    /// <c>--filter-method method1 --filter-method method2 ...</c>.
    /// </returns>
    public static string[] AddXunitPreExecutionFilterArguments(
        string[] args,
        ImmutableDictionary<int, ImmutableArray<string>> allureIdRegistry
    ) =>
        [
            ..args,
            ..GetXunitPreExecutionFilter(allureIdRegistry, TestPlan, Assembly.GetEntryAssembly()),
        ];

    /// <summary>
    /// Returns a sequence of filter arguments that, when passed to xUnit.net v3,
    /// enforces the provided test plan.
    /// </summary>
    /// <param name="allureIdRegistry">
    /// A mapping from Allure ID to test method names.
    /// </param>
    /// <param name="testPlan">
    /// An instance of the test plan. Use <see cref="AllureTestPlan.FromEnvironment"/> to read
    /// the global one or <see cref="TestPlan"/> to get the cached version.
    /// </param>
    /// <param name="testAssembly">A test assembly. In MTP flow, it's the entry assembly.</param>
    /// <returns>
    /// A sequence of xunit.v3.mtp-v2 arguments in form
    /// <c>--filter-method method1 --filter-method method2 ...</c>.
    /// </returns>
    public static IEnumerable<string> GetXunitPreExecutionFilter(
        ImmutableDictionary<int, ImmutableArray<string>> allureIdRegistry,
        AllureTestPlan testPlan,
        Assembly testAssembly
    )
    {
        if (ReferenceEquals(testPlan, AllureTestPlan.DEFAULT_TESTPLAN))
        {
            // No test plan provided.
            yield break;
        }

        bool emitted = false;

        var testAssemblyName = testAssembly.GetName().Name;
        foreach (var entry in testPlan.Tests)
        {
            if (entry.Selector is not null
                && TryMatchByFullName(testAssemblyName, entry.Selector, out var fullNameFilter))
            {
                emitted = true;
                yield return "--filter-method";
                yield return fullNameFilter;
            }

            if (entry.Id is not null
                && int.TryParse(entry.Id, out var expectedAllureId)
                && allureIdRegistry.TryGetValue(expectedAllureId, out var allureIdFilters))
            {
                foreach (var allureIdFilter in allureIdFilters)
                {
                    emitted = true;
                    yield return "--filter-method";
                    yield return allureIdFilter;
                }
            }
        }

        if (!emitted)
        {
            // The test plan exists but is empty or no entry matched.
            // No test should be run.
            yield return "--filter-method";
            yield return NON_EXISTING_METHOD_NAME;
        }
    }

    /// <summary>
    /// Skips the current test at runtime if the global test plan doesn't include it.
    /// This is a fallback test plan enforcement mechanism that prevents excluded tests from running
    /// if the pre-execution filtering doesn't work.
    /// </summary>
    /// <param name="testMethod">A test method that represents the current test.</param>
    public static void ApplyRuntimeGuard(MethodInfo testMethod) =>
        Assert.SkipUnless(
            TestPlan.IsSelected(
                fullName: IdFunctions.CreateFullName(testMethod),
                allureId: testMethod.GetCustomAttribute<AllureIdAttribute>()?.Value
            ),
            AllureTestPlan.SkipReason
        );

    static bool TryMatchByFullName(
        string testAssemblyName,
        string fullName,
        [NotNullWhen(true)] out string? filterMethodValue
    )
    {
        if (TrySplitAssemblyName(fullName, out var assemblyName, out var rest)
            && StringComparer.OrdinalIgnoreCase.Equals(assemblyName, testAssemblyName)
            && TryCropOutParameters(rest, out var methodFullName))
        {
            filterMethodValue = methodFullName;
            return true;
        }

        filterMethodValue = default;
        return false;
    }

    static bool TrySplitAssemblyName(
        string fullName,
        [NotNullWhen(true)] out string assemblyName,
        [NotNullWhen(true)] out string? rest
    )
    {
        if (fullName.IndexOf(':') is int assemblyNameEnd && assemblyNameEnd != -1)
        {
            assemblyName = fullName.Substring(0, assemblyNameEnd);
            rest = fullName.Substring(assemblyNameEnd + 1);
            return true;
        }

        rest = assemblyName = "";
        return false;
    }

    static bool TryCropOutParameters(
        string methodSignature,
        [NotNullWhen(true)] out string methodFullName
    )
    {
        int cropAt = methodSignature.IndexOf('(');
        int typeParametersStart = methodSignature.IndexOf('[');
        if (cropAt == -1 || typeParametersStart != -1 && typeParametersStart < cropAt)
        {
            cropAt = typeParametersStart;
        }

        if (cropAt == -1)
        {
            methodFullName = "";
            return false;
        }

        methodFullName = methodSignature.Substring(0, cropAt);
        return true;
    }

    const string NON_EXISTING_METHOD_NAME = "f78eccb2-459e-482b-87ea-d8d7106959d1";
}
