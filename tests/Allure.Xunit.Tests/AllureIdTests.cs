using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.Tests;

class AllureIdTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetAllureIdSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SetAllureIdFromTest,
            AllureSampleRegistry.SetAllureIdFromDispose,
            AllureSampleRegistry.AllureIdAttributeOnMethod,
            AllureSampleRegistry.LegacyAllureIdAttributeOnMethod,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetAllureIdSamples))]
    public async Task CheckAllureIdLabelIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var nodes = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        await Assert.That(nodes).Any(
            l =>
            {
                return (string)l["name"] == "ALLURE_ID" && (string)l["value"] == "1001";
            }
        );
    }
}
