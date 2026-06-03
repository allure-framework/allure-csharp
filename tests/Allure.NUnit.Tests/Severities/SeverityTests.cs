using Allure.Testing;

namespace Allure.NUnit.Tests.Severities;

class SeverityTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetSeveritySamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SetSeverityFromSetUp,
            AllureSampleRegistry.SetSeverityFromTest,
            AllureSampleRegistry.SetSeverityFromTearDown,
            AllureSampleRegistry.SeverityAttributeOnClass,
            AllureSampleRegistry.SeverityAttributeOnMethod,
            AllureSampleRegistry.SeverityAttributeOnBaseClass,
            AllureSampleRegistry.SeverityAttributeOnInterface,
            AllureSampleRegistry.LegacySeverityAttributeOnClass,
            AllureSampleRegistry.LegacySeverityAttributeOnMethod,
            AllureSampleRegistry.LegacySeverityAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetSeveritySamples))]
    public async Task CheckSuiteLabelsAreAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results).HasSingleTestResult()
            .With.SingleLabel("severity").With.Value("critical");
    }
}
