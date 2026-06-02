using Allure.Testing;

namespace Allure.Xunit.Tests.Names;

class NameTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetTestRenameSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SetTestNameFromTest,
            AllureSampleRegistry.SetTestNameFromDispose,
            AllureSampleRegistry.NameAttributeOnMethod,
            AllureSampleRegistry.XunitDisplayNameOnFact,
            AllureSampleRegistry.XunitDisplayNameOnTheory,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetTestRenameSamples))]
    public async Task CheckTestCanBeRenamed(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync2(sample);

        await Assert.That(results).HasSingleTestResult().With.Name("Lorem Ipsum");
    }

    [Test]
    public async Task MethodNameIsUsedForTheories()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.SingleTheory);

        await Assert.That(results).HasSingleTestResult().With.Name("TestMethod");
    }

    [Test]
    public async Task CheckAllureNameOnTestFixtureAffectsSuiteOnly()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.NameAttributeOnClass);

        await Assert.That(results).HasSingleTestResult()
            .With.Name("TestMethod")
            .With.SingleLabel("subSuite").With.Value("Lorem Ipsum");
    }
}