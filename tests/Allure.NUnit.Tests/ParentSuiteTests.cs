using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class ParentSuiteTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetParentSuiteSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.ParentSuiteAttributeOnClass,
            AllureSampleRegistry.ParentSuiteAttributeOnMethod,
            AllureSampleRegistry.ParentSuiteAttributeOnBaseClass,
            AllureSampleRegistry.ParentSuiteAttributeOnInterface,
            AllureSampleRegistry.AddParentSuiteFromSetUp,
            AllureSampleRegistry.AddParentSuiteFromTest,
            AllureSampleRegistry.AddParentSuiteFromTearDown,
            AllureSampleRegistry.LegacyParentSuiteAttributeOnClass,
            AllureSampleRegistry.LegacyParentSuiteAttributeOnMethod,
            AllureSampleRegistry.LegacyParentSuiteAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetParentSuiteSamples))]
    public async Task CheckParentSuiteIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "parentSuite" && (string)l["value"] == "foo";
            }
        );
    }
}
