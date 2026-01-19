using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

internal class LabelTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetCustomLabelSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> labelSamples = [
            AllureSampleRegistry.AttributeLabelOnClass,
            AllureSampleRegistry.AttributeLabelOnMethod,
            AllureSampleRegistry.AddLabelFromSetUp,
            AllureSampleRegistry.AddLabelFromTest,
            AllureSampleRegistry.AddLabelFromTearDown,
        ];

        return labelSamples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
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
