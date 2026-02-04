using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class CustomLabelTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetCustomLabelSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.LabelAttributeOnClass,
            AllureSampleRegistry.LabelAttributeOnMethod,
            AllureSampleRegistry.LabelAttributeOnBaseClass,
            AllureSampleRegistry.LabelAttributeOnInterface,
            AllureSampleRegistry.AddLabelFromSetUp,
            AllureSampleRegistry.AddLabelFromTest,
            AllureSampleRegistry.AddLabelFromTearDown,
            AllureSampleRegistry.LegacyLabelAttributeOnClass,
            AllureSampleRegistry.LegacyLabelAttributeOnMethod,
            AllureSampleRegistry.LegacyLabelAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetCustomLabelSamples))]
    public async Task CheckCustomLabelIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "foo" && (string)l["value"] == "bar";
            }
        );
    }
}
