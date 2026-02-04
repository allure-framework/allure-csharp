using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class OwnerTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetOwnerSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.NUnitAuthorPropertyOnTest,
            AllureSampleRegistry.NUnitAuthorPropertyOnTestCase,
            AllureSampleRegistry.NUnitAuthorPropertyOnTestFixture,
            AllureSampleRegistry.OwnerAttributeOnBaseClass,
            AllureSampleRegistry.OwnerAttributeOnClass,
            AllureSampleRegistry.OwnerAttributeOnInterface,
            AllureSampleRegistry.OwnerAttributeOnMethod,
            AllureSampleRegistry.SetOwnerFromSetUp,
            AllureSampleRegistry.SetOwnerFromTearDown,
            AllureSampleRegistry.SetOwnerFromTest,
            AllureSampleRegistry.LegacyOwnerAttributeOnClass,
            AllureSampleRegistry.LegacyOwnerAttributeOnMethod,
            AllureSampleRegistry.LegacyOwnerAttributeOnBaseClass,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.SampleId));
    }

    [Test]
    [MethodDataSource(nameof(GetOwnerSamples))]
    public async Task CheckOwnerIsAdded(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var labels = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        var owner = labels.First(static (l) => (string)l["name"] == "owner");
        await Assert.That((string)owner["value"]).IsEqualTo("John Doe");
    }
}
