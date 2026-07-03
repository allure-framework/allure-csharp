using Allure.Testing;
using Allure.Testing.Execution;

namespace Allure.Xunit.Tests.AllureIds;

class AllureIdTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetAllureIdSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SetAllureIdFromTest,
            AllureSampleRegistry.SetAllureIdFromDispose,
            AllureSampleRegistry.AllureIdAttributeOnMethod,
            AllureSampleRegistry.LegacyAllureIdAttributeOnMethod,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetAllureIdSamples))]
    public async Task CheckAllureIdLabelIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results).HasSingleTestResult()
            .That.HasSingleLabel("ALLURE_ID")
            .With.Value("1001");
    }
}
