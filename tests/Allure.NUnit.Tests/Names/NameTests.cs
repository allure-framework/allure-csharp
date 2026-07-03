using Allure.Testing;
using Allure.Testing.Execution;

namespace Allure.NUnit.Tests.Names;

class NameTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetTestRenameSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SetTestNameFromSetUp,
            AllureSampleRegistry.SetTestNameFromTest,
            AllureSampleRegistry.SetTestNameFromTearDown,
            AllureSampleRegistry.LegacyNameAttribute,
            AllureSampleRegistry.NameAttributeOnMethod,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetTestRenameSamples))]
    public async Task CheckTestCanBeRenamed(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results).HasSingleTestResult().With.Name("Lorem Ipsum");
    }

    [Test]
    public async Task MethodNameIsUsedForTestCases()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SingleTescCase);

        await Assert.That(results).HasSingleTestResult().With.Name("TestMethod");
    }

    [Test]
    public async Task CheckAllureNameOnTestFixtureAffectsSuiteOnly()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.NameAttributeOnClass);

        await Assert.That(results).HasSingleTestResult()
            .With.Name("TestMethod")
            .With.SingleLabel("subSuite").With.Value("Lorem Ipsum");
    }
}