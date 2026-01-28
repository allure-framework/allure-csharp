using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class SuiteHierarchyTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetSuiteHierarchySamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SuiteHierarchyAttributeOnClass,
            AllureSampleRegistry.SuiteHierarchyAttributeOnMethod,
            AllureSampleRegistry.SuiteHierarchyAttributeOnBaseClass,
            AllureSampleRegistry.SuiteHierarchyAttributeOnInterface,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetSuiteHierarchySamples))]
    public async Task CheckSuiteLabelsAreAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            static (l) =>
            {
                return (string)l["name"] == "parentSuite" && (string)l["value"] == "foo";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "suite" && (string)l["value"] == "bar";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "subSuite" && (string)l["value"] == "baz";
            }
        );
    }
}
