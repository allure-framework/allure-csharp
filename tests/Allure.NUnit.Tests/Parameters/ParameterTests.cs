using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.NUnit.Tests.Parameters;

class ParameterTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetParameterSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.AddTestParameter,
            AllureSampleRegistry.ParameterAttributesOnTestCaseParameters,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetParameterSamples))]
    public async Task AddTestParameterApiWorks(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results).HasSingleTestResult()
            .With.ParametersMatching([
                p => p.HasName("name1")
                    .And.HasValue("\"value-1\"")
                    .And.HasNoMode()
                    .And.HasExcluded(false),
                p => p.HasName("name2")
                    .And.HasValue("\"value-2\"")
                    .And.HasMode(AllureParameterMode.Masked)
                    .And.HasExcluded(false),
                p => p.HasName("name3")
                    .And.HasValue("\"value-3\"")
                    .And.HasMode(AllureParameterMode.Hidden)
                    .And.HasExcluded(false),
                p => p.HasName("name4")
                    .And.HasValue("\"value-4\"")
                    .And.HasNoMode()
                    .And.HasExcluded(true),
                p => p.HasName("name5")
                    .And.HasValue("\"value-5\"")
                    .And.HasMode(AllureParameterMode.Masked)
                    .And.HasExcluded(true),
            ]);
    }
}