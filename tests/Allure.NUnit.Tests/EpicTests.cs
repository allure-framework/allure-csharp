using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class EpicTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetEpicSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.LegacyEpicAttributeOnClass,
            AllureSampleRegistry.LegacyEpicAttributeOnMethod,
            AllureSampleRegistry.LegacyEpicAttributeOnBaseClass,
            AllureSampleRegistry.AddEpicFromSetUp,
            AllureSampleRegistry.AddEpicFromTest,
            AllureSampleRegistry.AddEpicFromTearDown,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetEpicSamples))]
    public async Task CheckEpicIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "epic" && (string)l["value"] == "foo";
            }
        );
    }
}
