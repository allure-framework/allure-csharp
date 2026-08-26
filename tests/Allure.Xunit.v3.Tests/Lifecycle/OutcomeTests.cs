using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Lifecycle;

class OutcomeTests
{
    readonly static AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.Outcomes, token);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckPassingFactIsRecordedAsPassed()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Lifecycle.Outcomes.TestClass.PassingFact"
        )
            .With.Status(AllureStatus.Passed);
    }

    [Test]
    public async Task CheckAssertionFailureIsRecordedAsFailed()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Lifecycle.Outcomes.TestClass.FailingFact"
        )
            .With.Status(AllureStatus.Failed)
            .With.StatusDetails(
                (sd) => sd
                    .HasMessage((m) => m.Contains("Expected:").And.Contains("Actual:"))
                    .And.HasTrace((t) => t.Contains("Xunit.MicrosoftTestingPlatform.XunitException"))
            );
    }

    [Test]
    public async Task CheckUnexpectedExceptionIsRecordedAsBroken()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Lifecycle.Outcomes.TestClass.BrokenFact"
        )
            .With.Status(AllureStatus.Broken)
            .With.StatusDetails(
                (sd) => sd
                    .HasMessage((m) => m.Contains("Something went wrong."))
                    .And.HasTrace((t) => t.Contains("Xunit.MicrosoftTestingPlatform.XunitException")
                        .And.Contains("InvalidOperationException"))
            );
    }

    [Test]
    public async Task CheckSkippedFactIsRecordedAsSkipped()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Lifecycle.Outcomes.TestClass.SkippedFact"
        )
            .With.Status(AllureStatus.Skipped)
            .With.StatusDetails(
                (sd) => sd.HasMessage(
                    (m) => m.Contains("Not part of this run.")
                )
            );
    }
}
