using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

internal class StoryTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetStorySamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.StoryAttributeOnClass,
            AllureSampleRegistry.StoryAttributeOnMethod,
            AllureSampleRegistry.StoryAttributeOnBaseClass,
            AllureSampleRegistry.AddStoryFromSetUp,
            AllureSampleRegistry.AddStoryFromTest,
            AllureSampleRegistry.AddStoryFromTearDown,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetStorySamples))]
    public async Task CheckStoryIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "story" && (string)l["value"] == "foo";
            }
        );
    }
}
