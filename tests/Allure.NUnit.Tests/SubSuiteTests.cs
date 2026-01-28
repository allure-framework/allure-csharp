using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class SubSuiteTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetSubSuiteSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SubSuiteAttributeOnClass,
            AllureSampleRegistry.SubSuiteAttributeOnMethod,
            AllureSampleRegistry.SubSuiteAttributeOnBaseClass,
            AllureSampleRegistry.SubSuiteAttributeOnInterface,
            AllureSampleRegistry.AddSubSuiteFromSetUp,
            AllureSampleRegistry.AddSubSuiteFromTest,
            AllureSampleRegistry.AddSubSuiteFromTearDown,
            AllureSampleRegistry.LegacySubSuiteAttributeOnClass,
            AllureSampleRegistry.LegacySubSuiteAttributeOnMethod,
            AllureSampleRegistry.LegacySubSuiteAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetSubSuiteSamples))]
    public async Task CheckSubSuiteIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "subSuite" && (string)l["value"] == "foo";
            }
        );
    }
}
