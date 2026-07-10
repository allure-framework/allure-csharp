using Allure.Testing;

namespace Allure.Xunit.v3.Tests.Generator;

class GeneratorTests
{
    [Test]
    public async Task ShouldReportRawResultsWhenAllureAttributeNotGenerated(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.ApplyAttributeDisabled,
            token
        );

        var testResult = await Assert.That(results).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Generator.ApplyAttributeDisabled.TestClass.TestMethod(argument: \"value-1\")"
        );

        await Assert.That(testResult).HasNoLabel("tag")
            .And.HasParametersMatching([]);
    }

    [Test]
    public async Task ManualAssemblyAttributeShouldSuppressGeneratedDuplicateAndReportResults(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.ManualAssemblyAttribute,
            token
        );

        await Assert.That(results.TestResults).Count().IsEqualTo(2);
    }

    [Test]
    public async Task SelfRegistrationDisabledShouldNotReportResultsByDefault(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.SelfRegistrationDisabled,
            token
        );

        await Assert.That(results.TestResults).Count().IsEqualTo(0);
        await Assert.That(results.Containers).Count().IsEqualTo(0);
        await Assert.That(results.Globals).Count().IsEqualTo(0);
    }

    [Test]
    public async Task SelfRegistrationDisabledShouldAllowCustomRunnerReporter(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.CustomRunnerReporter, token);

        await Assert.That(results).HasSingleGlobals()
            .With.SingleError()
            .That.HasMessage("custom reporter works");
        await Assert.That(results.TestResults).Count().IsEqualTo(0);
    }
}
