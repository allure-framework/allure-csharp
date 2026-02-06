using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.Xunit.Tests;

class NameTests
{
    public static IEnumerable<TestDataRow<AllureSampleRegistryEntry>> GetTestRenameSamples()
    {
        IEnumerable<AllureSampleRegistryEntry> samples = [
            AllureSampleRegistry.SetTestNameFromTest,
            AllureSampleRegistry.SetTestNameFromDispose,
            AllureSampleRegistry.NameAttributeOnMethod,
            AllureSampleRegistry.XunitDisplayNameOnFact,
            AllureSampleRegistry.XunitDisplayNameOnTheory,
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
    public async Task MethodNameIsUsedForTheories()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.SingleTheory);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        await Assert.That((string)results.TestResults[0]["name"]).IsEqualTo("TestMethod");
    }

    [Test]
    public async Task CheckAllureNameOnTestFixtureAffectsSuiteOnly()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.NameAttributeOnClass);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);
        var testResult = results.TestResults[0];
        await Assert.That((string)testResult["name"]).IsEqualTo("TestMethod");
        var labels = testResult["labels"].AsArray().Cast<JsonObject>();
        var subSuiteLabel = labels.First(static (l) => (string)l["name"] == "subSuite");
        await Assert.That((string)subSuiteLabel["value"]).IsEqualTo("Lorem Ipsum");
        await Assert.That(labels).Any(
            static (l) => (string)l["name"] == "parentSuite"
        ).And.Any(
            static (l) => (string)l["name"] == "suite"
        );
    }
}