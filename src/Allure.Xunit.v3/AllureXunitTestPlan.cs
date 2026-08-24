using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using Allure.Sdk.Functions;
using Allure.Sdk.TestPlan;

namespace Allure.Xunit;

/// <summary>
/// Provides helpers used to apply Allure test plans to xUnit.net v3 test runs.
/// </summary>
public static class AllureXunitTestPlan
{
    class TestPlanHolder(AllureTestPlan? value)
    {
        public AllureTestPlan? Value => value;
    }

    static TestPlanHolder? testPlan = null;

    /// <summary>
    /// Gets the most recently loaded test plan. If no test plan is loaded,
    /// loads it first by calling <see cref="Reload"/>.
    /// </summary>
    public static AllureTestPlan? Current =>
        testPlan is { Value: var value }
            ? value
            : Reload();

    /// <summary>
    /// Loads a fresh test plan from the file pointed by the
    /// <c>ALLURE_TESTPLAN_PATH</c>
    /// environment variable.
    /// </summary>
    /// <returns>The loaded test plan.</returns>
    public static AllureTestPlan? Reload()
    {
        var value = AllureTestPlan.FromEnvironment();
        Volatile.Write(ref testPlan, new(value));
        return value;
    }

    /// <summary>
    /// Returns an array consisting of the original CLI arguments plus filter arguments that,
    /// when passed to xUnit.net v3, enforces the test plan for the entry point assembly.
    /// </summary>
    /// <remarks>
    /// This function always loads a fresh test plan.
    /// </remarks>
    /// <param name="originalArguments">An array of command-line arguments passed to the test application.</param>
    /// <param name="allureIdRegistry">
    /// A mapping from Allure ID to test method names.
    /// </param>
    /// <returns>
    /// A new array that contains <paramref name="originalArguments"/> followed by
    /// the xUnit.net pre-execution filter arguments for the current Allure test plan.
    /// </returns>
    public static string[] AddXunitPreExecutionFilterArguments(
        string[] originalArguments,
        ImmutableDictionary<int, ImmutableArray<string>> allureIdRegistry
    ) =>
        [
            ..originalArguments,
            ..GetXunitPreExecutionFilter(
                allureIdRegistry,
                Reload(),
                Assembly.GetEntryAssembly()
                    ?? throw new InvalidOperationException(
                        "Could not get the entry assembly."
                    )
            ),
        ];

    /// <summary>
    /// Returns a sequence of filter arguments that, when passed to xUnit.net v3,
    /// enforces the provided test plan.
    /// </summary>
    /// <param name="allureIdRegistry">
    /// A mapping from Allure ID to test method names.
    /// </param>
    /// <param name="testPlan">
    /// The test plan to enforce. Use <see cref="Reload"/> to read and cache
    /// the global test plan, or use <see cref="Current"/> to get the cached instance.
    /// </param>
    /// <param name="testAssembly">
    /// The test assembly. In the Microsoft Testing Platform flow, this is the entry assembly.
    /// </param>
    /// <returns>
    /// A sequence of xunit.v3.mtp-v2 arguments in the form
    /// <c>--filter-method method1 --filter-method method2 ...</c>.
    /// </returns>
    public static IEnumerable<string> GetXunitPreExecutionFilter(
        ImmutableDictionary<int, ImmutableArray<string>> allureIdRegistry,
        AllureTestPlan? testPlan,
        Assembly testAssembly
    )
    {
        if (testPlan is null)
        {
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
    /// Checks if the test defined by the provided method is selected
    /// by the global test plan.
    /// </summary>
    /// <param name="testMethod">A test method to check.</param>
    /// <returns>
    /// <see langword="true"/> if the method is selected by the current test plan
    /// or no test plan is defined;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsSelected(MethodInfo testMethod) =>
        Current?.IsSelected(
            fullName: ReflectionNames.ForMethod(testMethod),
            allureId: testMethod.GetCustomAttribute<AllureIdAttribute>()?.Value
        ) ?? true;

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
