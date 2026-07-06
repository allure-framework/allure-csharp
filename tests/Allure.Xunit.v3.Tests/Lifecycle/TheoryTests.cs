using System.Collections.Immutable;
using Allure.Testing;
using Allure.Testing.Assertions.Model;
using TUnit.Assertions.Enums;

namespace Allure.Xunit.v3.Tests.Lifecycle;

class TheoryTests
{
    readonly static AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.Theories, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(3);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckResultWithParametersRecordedForEachRow()
    {
        var expectedFullName = "Allure.Xunit.v3.Tests.Samples.Lifecycle.Theories:"
            + "Allure.Xunit.v3.Tests.Samples.Lifecycle.Theories."
            + "TestClass.TestMethod(System.String)";
        ImmutableArray<string> expectedTitlePath = [
            "Allure.Xunit.v3.Tests.Samples.Lifecycle.Theories",
            "Allure",
            "Xunit",
            "v3",
            "Tests",
            "Samples",
            "Lifecycle",
            "Theories",
            "TestClass",
            "TestMethod(System.String)",
        ];

        await Assert.That(results.Value).HasTestResults([
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.Lifecycle.Theories.TestClass.TestMethod(value: \"first\")")
                .And.HasFullName(expectedFullName)
                .And.HasTitlePath((p) => p.IsEquivalentTo(expectedTitlePath, CollectionOrdering.Matching))
                .And.HasParametersMatching([
                    (p) => p.HasName("value").And.HasValue("\"first\""),
                ]),
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.Lifecycle.Theories.TestClass.TestMethod(value: \"second\")")
                .And.HasFullName(expectedFullName)
                .And.HasTitlePath((p) => p.IsEquivalentTo(expectedTitlePath, CollectionOrdering.Matching))
                .And.HasParametersMatching([
                    (p) => p.HasName("value").And.HasValue("\"second\""),
                ]),
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.Lifecycle.Theories.TestClass.TestMethod(value: \"third\")")
                .And.HasFullName(expectedFullName)
                .And.HasTitlePath((p) => p.IsEquivalentTo(expectedTitlePath, CollectionOrdering.Matching))
                .And.HasParametersMatching([
                    (p) => p.HasName("value").And.HasValue("\"third\""),
                ]),
        ]);
    }

    [Test]
    public async Task CheckTheoryRowsShareTestCaseId()
    {
        var testCaseId = await Assert.That(results.Value).HasTestResultAt(0).With.TestCaseId();

        await Assert.That(results.Value).HasTestResults([
            (tr) => tr.HasTestCaseId(testCaseId),
            (tr) => tr.HasTestCaseId(testCaseId),
            (tr) => tr.HasTestCaseId(testCaseId),
        ]);
    }

    [Test]
    public async Task CheckTheoryRowsHaveDifferentHistoryIds()
    {
        ImmutableHashSet<string> values = [
            await Assert.That(results.Value).HasTestResultAt(0).With.HistoryId(),
            await Assert.That(results.Value).HasTestResultAt(1).With.HistoryId(),
            await Assert.That(results.Value).HasTestResultAt(2).With.HistoryId(),
        ];

        await Assert.That(values).Count().IsEqualTo(3);
    }
}
