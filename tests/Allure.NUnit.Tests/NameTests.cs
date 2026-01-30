using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class NameTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetTestRenameSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SetTestNameFromSetUp,
            AllureSampleRegistry.SetTestNameFromTest,
            AllureSampleRegistry.SetTestNameFromTearDown,
            AllureSampleRegistry.LegacyNameAttribute,
            AllureSampleRegistry.NameAttributeOnMethod,
        ];

        return samples.Select(static (sample) =>
            new TestDataRow<AllureSampleRegistryEntry>(sample, DisplayName: sample.Id));
    }

    [Test]
    [MethodDataSource(nameof(GetTestRenameSamples))]
    public async Task CheckTestCanBeRenamed(AllureSampleRegistryEntry sample)
    {
        var results = await AllureSampleRunner.RunAsync(sample);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["name"]).IsEqualTo("Lorem Ipsum");
    }

    [Test]
    public async Task MethodNameIsUsedForTestCases()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SingleTescCase);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["name"]).IsEqualTo("TestMethod");
    }

    [Test]
    public async Task CheckAllureNameAffectsSuite()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.NameAttributeOnClass);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var labels = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        var subSuiteLabel = labels.First(static (l) => (string)l["name"] == "subSuite");
        await Assert.That((string)subSuiteLabel["value"]).IsEqualTo("Lorem Ipsum");
    }
}