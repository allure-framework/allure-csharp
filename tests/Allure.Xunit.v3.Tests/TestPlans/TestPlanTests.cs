using Allure.Testing;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Execution;

namespace Allure.Xunit.v3.Tests.TestPlans;

class TestPlanTests
{
    [Test]
    public async Task TestPlanSelectorShouldFilterTests(CancellationToken token)
    {
        var fullName = "Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan:"
            + "Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan.TestClass.SecondTest()";
        var results = await RunWithTestPlan(
            $$"""
            {
              "tests": [
                {
                  "selector": "{{fullName}}"
                }
              ]
            }
            """,
            token
        );

        await Assert.That(results).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan.TestClass.SecondTest"
        );
    }

    [Test]
    public async Task TestPlanAllureIdShouldFilterTests(CancellationToken token)
    {
        var results = await RunWithTestPlan(
            """{"tests":[{"id":"3001"}]}""",
            token
        );

        await Assert.That(results).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan.TestClass.AllureIdTest"
        )
            .With.SingleLabel("ALLURE_ID")
            .That.HasValue("3001");
    }

    [Test]
    public async Task AllureIdPreFilterShouldAvoidConstructingUnselectedTests(CancellationToken token)
    {
        var results = await RunWithTestPlan(
            """{"tests":[{"id":"3002"}]}""",
            AllureSampleRegistry.AllureIdPreFilter,
            token
        );

        await Assert.That(results).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.TestPlans.AllureIdPreFilter.SelectedMarkerClass.TestMethod"
        )
            .With.SingleLabel("ALLURE_ID")
            .That.HasValue("3002");

        await Assert.That(results).HasSingleGlobals()
            .With.SingleError()
            .That.HasMessage("selected Allure ID test was constructed");
    }

    [Test]
    public async Task UnmatchedTestPlanShouldProduceNoResults(CancellationToken token)
    {
        var results = await RunWithTestPlan(
            """{"tests":[{"id":"404"}]}""",
            token
        );

        await Assert.That(results.TestResults).Count().IsEqualTo(0);
        await Assert.That(results.Containers).Count().IsEqualTo(0);
        await Assert.That(results.Globals).Count().IsEqualTo(0);
    }

    [Test]
    public async Task TestPlanSelectorWithParametersShouldBeCroppedForXunitFilter(CancellationToken token)
    {
        var fullName = "Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan:"
            + "Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan.TestClass.ParameterizedTheory(System.String)";

        var results = await RunWithTestPlan(
            $$"""{"tests":[{"selector":"{{fullName}}"}]}""",
            token
        );

        await Assert.That(results.TestResults).Count().IsEqualTo(2);
        await Assert.That(results)
            .HasTestResults([
                (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan.TestClass.ParameterizedTheory(value: \"foo\")"),
                (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan.TestClass.ParameterizedTheory(value: \"bar\")"),
            ]);
    }

    [Test]
    public async Task TestPlanSelectorWithTypeParametersShouldBeCroppedForXunitFilter(CancellationToken token)
    {
        var fullName = "Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan:"
            + "Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan.TestClass.GenericTheory[T](T)";

        var results = await RunWithTestPlan(
            $$"""{"tests":[{"selector":"{{fullName}}"}]}""",
            token
        );

        await Assert.That(results).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.TestPlans.TestPlan.TestClass.GenericTheory<String>(value: \"baz\")"
        )
            .With.FullName(fullName);
    }

    static async Task<AllureResults> RunWithTestPlan(
        string testPlanJson,
        CancellationToken token
    ) =>
        await RunWithTestPlan(testPlanJson, AllureSampleRegistry.TestPlan, token);

    static async Task<AllureResults> RunWithTestPlan(
        string testPlanJson,
        AllureSampleRegistryEntry sample,
        CancellationToken token
    )
    {
        var testPlanPath = Path.Combine(
            Path.GetTempPath(),
            $"allure-testplan-{Guid.NewGuid():N}.json"
        );

        await File.WriteAllTextAsync(testPlanPath, testPlanJson, token);
        try
        {
            return await AllureSampleRunner.RunAsync(sample, new()
            {
                EnvironmentVariables = { ["ALLURE_TESTPLAN_PATH"] = testPlanPath },
            }, token);
        }
        finally
        {
            File.Delete(testPlanPath);
        }
    }
}
