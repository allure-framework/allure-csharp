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
}
