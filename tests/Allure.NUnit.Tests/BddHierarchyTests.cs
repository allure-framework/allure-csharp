using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class BddHierarchyTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetBddHierarchySamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.BddHierarchyAttributeOnClass,
            AllureSampleRegistry.BddHierarchyAttributeOnMethod,
            AllureSampleRegistry.BddHierarchyAttributeOnBaseClass,
            AllureSampleRegistry.BddHierarchyAttributeOnInterface,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetBddHierarchySamples))]
    public async Task CheckBddLabelsAreAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            static (l) =>
            {
                return (string)l["name"] == "epic" && (string)l["value"] == "foo";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "feature" && (string)l["value"] == "bar";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "story" && (string)l["value"] == "baz";
            }
        );
    }
}
