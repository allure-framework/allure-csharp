using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Lifecycle;

class FailureTests
{
    readonly static AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.Failures, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(2);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task ShouldReportClassConstructorException()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Lifecycle.Failures.BadClass.TestMethod"
        )
            .With.Status(AllureStatus.Broken)
            .With.StatusDetails((sd) => sd.HasMessage((m) => m.Contains("Constructor exploded.")));
    }

    [Test]
    public async Task ShouldReportTheoryDataException()
    {
        await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Lifecycle.Failures.GoodClass.BadTheory"
        )
            .With.Status(AllureStatus.Broken)
            .With.StatusDetails((sd) => sd.HasMessage((m) => m.Contains("Data source exploded.")));
    }
}
