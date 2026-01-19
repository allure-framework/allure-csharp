using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

internal class FeatureTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetFeatureSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.FeatureAttributeOnClass,
            AllureSampleRegistry.FeatureAttributeOnMethod,
            AllureSampleRegistry.FeatureAttributeOnBaseClass,
            AllureSampleRegistry.AddFeatureFromSetUp,
            AllureSampleRegistry.AddFeatureFromTest,
            AllureSampleRegistry.AddFeatureFromTearDown,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetFeatureSamples))]
    public async Task CheckFeatureIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "feature" && (string)l["value"] == "foo";
            }
        );
    }
}
