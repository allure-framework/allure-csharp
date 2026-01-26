using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class SuiteTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetSuiteSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SuiteAttributeOnClass,
            AllureSampleRegistry.SuiteAttributeOnMethod,
            AllureSampleRegistry.SuiteAttributeOnBaseClass,
            AllureSampleRegistry.AddSuiteFromSetUp,
            AllureSampleRegistry.AddSuiteFromTest,
            AllureSampleRegistry.AddSuiteFromTearDown,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetSuiteSamples))]
    public async Task CheckSuiteIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "suite" && (string)l["value"] == "foo";
            }
        );
    }
}
